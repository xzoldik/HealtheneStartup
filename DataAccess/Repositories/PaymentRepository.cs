// PaymentService.cs
using Domain.Dtos.PaymentDtos;
using Domain.Globals;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class PaymentRepository : IPaymentRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ChargilySettings _settings;
    private readonly ILogger<PaymentRepository> _logger;

    public PaymentRepository(
        IHttpClientFactory httpClientFactory,
        IOptions<ChargilySettings> settings,
        ILogger<PaymentRepository> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    // Method to create the payment
    public async Task<(int paymentId, string? checkoutUrl)?> CreatePaymentAsync(CreatePaymentRequest request)
    {
        int newPaymentId;

        // 1. Insert a "Pending" payment record into the database using ADO.NET
        string insertSql = @"
            INSERT INTO Payments (Status, Amount, Currency, CreatedAt,SessionID,PatientID)
            VALUES (@Status, @Amount, @Currency, @CreatedAt,@SessionID,@PatientID);
            SELECT SCOPE_IDENTITY();"; // This command gets the ID of the new row

        using (var connection = new SqlConnection(Connection.ConnectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(insertSql, connection);
            command.Parameters.AddWithValue("@Status", "Pending");
            command.Parameters.AddWithValue("@Amount", request.Amount);
            command.Parameters.AddWithValue("@Currency", request.Currency);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@SessionID", request.SessionID);
            command.Parameters.AddWithValue("@PatientID", request.PatientID);

            var result = await command.ExecuteScalarAsync();
            newPaymentId = Convert.ToInt32(result);
        }

        // 2. Prepare the data to send to Chargily
        var payload = new
        {
            amount = (int)request.Amount,
            currency = request.Currency,
            success_url = _settings.success_url,
            webhook_endpoint = _settings.WebhookUrl,
            metadata = new { localPaymentId = newPaymentId }
        };

        // 3. Make the API call to Chargily
        var client = _httpClientFactory.CreateClient("ChargilyClient");
        var response = await client.PostAsJsonAsync(_settings.ApiUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Chargily API call failed: {Error}", await response.Content.ReadAsStringAsync());
            return null;
        }

        // 4. Process the successful response
        var chargilyResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
        string? checkoutUrl = chargilyResponse.TryGetProperty("checkout_url", out var urlElem) ? urlElem.GetString() : null;
        string? checkoutId = chargilyResponse.TryGetProperty("id", out var idElem) ? idElem.GetString() : null;

        // 5. Update our database record with the checkout URL and ID from Chargily
        string updateSql = @"
            UPDATE Payments SET ChargilyCheckoutId = @CheckoutId, ChargilyCheckoutUrl = @CheckoutUrl
            WHERE PaymentId = @PaymentId;";

        using (var connection = new SqlConnection(Connection.ConnectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(updateSql, connection);
            command.Parameters.AddWithValue("@CheckoutId", checkoutId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CheckoutUrl", checkoutUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PaymentId", newPaymentId);
            await command.ExecuteNonQueryAsync();
        }

        return (newPaymentId, checkoutUrl);
    }

    // Method for the client to check the payment status
    public async Task<string?> GetPaymentStatusAsync(int paymentId)
    {
        string? status = null;
        string sql = "SELECT Status FROM Payments WHERE PaymentId = @PaymentId";

        using (var connection = new SqlConnection(Connection.ConnectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@PaymentId", paymentId);

            var result = await command.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
            {
                status = result.ToString();
            }
        }
        return status;
    }

    // Method to process incoming webhooks from Chargily
    public async Task ProcessWebhookAsync(string jsonPayload, string signatureHeader)
    {
        _logger.LogInformation("Received webhook payload: {Payload}", jsonPayload);
        _logger.LogInformation("Received signature header: {Signature}", signatureHeader);

        // 1. Verify the signature (CRITICAL for security)
        if (!IsSignatureValid(jsonPayload, signatureHeader))
        {
            _logger.LogWarning("Invalid webhook signature received. Expected signature validation failed.");
            return; // Stop processing
        }

        _logger.LogInformation("Webhook signature validated successfully.");

        try
        {
            // 2. Parse the webhook data
            var webhookData = JsonSerializer.Deserialize<JsonElement>(jsonPayload);

            // Log the entire webhook structure for debugging
            _logger.LogInformation("Webhook data structure: {WebhookData}", webhookData.ToString());

            var eventType = webhookData.TryGetProperty("type", out var typeElem) ? typeElem.GetString() : null;
            _logger.LogInformation("Event type: {EventType}", eventType);

            if (!webhookData.TryGetProperty("data", out var dataObject))
            {
                _logger.LogWarning("Webhook data does not contain 'data' property");
                return;
            }

            if (!dataObject.TryGetProperty("metadata", out var metadata))
            {
                _logger.LogWarning("Webhook data does not contain 'metadata' property");
                return;
            }

            if (!metadata.TryGetProperty("localPaymentId", out var paymentIdElem))
            {
                _logger.LogWarning("Webhook metadata does not contain 'localPaymentId' property");
                return;
            }

            int paymentId = paymentIdElem.GetInt32();
            _logger.LogInformation("Processing webhook for payment ID: {PaymentId}", paymentId);

            string newStatus = "Updated via Webhook";

            // Handle different event types
            switch (eventType)
            {
                case "checkout.paid":
                    newStatus = "Paid";
                    _logger.LogInformation("Payment ID {PaymentId} was paid.", paymentId);
                    break;

                case "checkout.failed":
                    newStatus = "Failed";
                    _logger.LogWarning("Payment ID {PaymentId} failed.", paymentId);
                    break;

                case "checkout.expired":
                    newStatus = "Expired";
                    _logger.LogInformation("Payment ID {PaymentId} expired.", paymentId);
                    break;

                case "checkout.cancelled":
                    newStatus = "Cancelled";
                    _logger.LogInformation("Payment ID {PaymentId} was cancelled.", paymentId);
                    break;

                default:
                    _logger.LogInformation("Unhandled event type: {EventType} for payment ID {PaymentId}", eventType, paymentId);
                    newStatus = $"Event: {eventType}";
                    break;
            }

            // 3. Update the database record with the new status
            string updateSql = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";

            using (var connection = new SqlConnection(Connection.ConnectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(updateSql, connection);
                command.Parameters.AddWithValue("@Status", newStatus);
                command.Parameters.AddWithValue("@PaymentId", paymentId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                _logger.LogInformation("Updated {RowsAffected} payment record(s) with status {Status} for payment ID {PaymentId}",
                    rowsAffected, newStatus, paymentId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook payload: {Payload}", jsonPayload);
            throw; // Re-throw to return 500 status to Chargily
        }
    }

    private bool IsSignatureValid(string payload, string signatureHeader)
    {
        if (string.IsNullOrEmpty(_settings.ApiSecret))
        {
            _logger.LogWarning("API secret is not configured. Skipping signature validation.");
            return true; // Allow in development, but log warning
        }

        try
        {
            // Chargily uses the API secret key to sign the webhook payload
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(_settings.ApiSecret);
            byte[] messageBytes = encoding.GetBytes(payload);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hash = hmac.ComputeHash(messageBytes);
                string computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                _logger.LogInformation("Computed signature: {ComputedSignature}", computedSignature);
                _logger.LogInformation("Received signature: {ReceivedSignature}", signatureHeader.ToLower());

                bool isValid = computedSignature.Equals(signatureHeader.ToLower(), StringComparison.OrdinalIgnoreCase);

                if (!isValid)
                {
                    // Try alternative signature format (base64)
                    string base64Signature = Convert.ToBase64String(hash);
                    _logger.LogInformation("Trying base64 signature: {Base64Signature}", base64Signature);
                    isValid = base64Signature.Equals(signatureHeader, StringComparison.OrdinalIgnoreCase);
                }

                return isValid;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating webhook signature");
            return false;
        }
    }
}