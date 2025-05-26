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
            command.Parameters.AddWithValue("@PatientID", request.PatientID); // Assuming you want to store this too
            // Execute the command and get the new ID
            var result = await command.ExecuteScalarAsync();
            newPaymentId = Convert.ToInt32(result);
        }

        // 2. Prepare the data to send to Chargily.
        // NO success_url or failure_url. Webhook is now mandatory.
        var payload = new
        {
            amount = (int)request.Amount,
            currency = request.Currency,
            success_url = _settings.success_url, // ADD THIS LINE
            webhook_endpoint = _settings.WebhookUrl,
            metadata = new { localPaymentId = newPaymentId }
        };

        // 3. Make the API call to Chargily
        var client = _httpClientFactory.CreateClient("ChargilyClient");
        var response = await client.PostAsJsonAsync(_settings.ApiUrl, payload);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Chargily API call failed: {Error}", await response.Content.ReadAsStringAsync());
            // In a real app, you might want to update the DB record to "Failed" here.
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
        // 1. Verify the signature (CRITICAL for security)
        if (!IsSignatureValid(jsonPayload, signatureHeader))
        {
            _logger.LogWarning("Invalid webhook signature received.");
            return; // Stop processing
        }

        _logger.LogInformation("Webhook signature validated successfully.");

        // 2. Parse the webhook data
        var webhookData = JsonSerializer.Deserialize<JsonElement>(jsonPayload);
        var eventType = webhookData.TryGetProperty("type", out var typeElem) ? typeElem.GetString() : null;
        var dataObject = webhookData.GetProperty("data");
        var metadata = dataObject.GetProperty("metadata");
        int paymentId = metadata.GetProperty("localPaymentId").GetInt32();

        string newStatus = "Updated via Webhook";

        if (eventType == "checkout.paid")
        {
            newStatus = "Paid";
            _logger.LogInformation("Payment ID {PaymentId} was paid.", paymentId);
        }
        else if (eventType == "checkout.failed")
        {
            newStatus = "Failed";
            _logger.LogWarning("Payment ID {PaymentId} failed.", paymentId);
        }

        // 3. Update the database record with the new status
        string updateSql = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId AND Status = 'Pending'";
        using (var connection = new SqlConnection(Connection.ConnectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand(updateSql, connection);
            command.Parameters.AddWithValue("@Status", newStatus);
            command.Parameters.AddWithValue("@PaymentId", paymentId);
            await command.ExecuteNonQueryAsync();
        }
    }

    private bool IsSignatureValid(string payload, string signatureHeader)
    {
        if (string.IsNullOrEmpty(_settings.WebhookSecret))
        {
            _logger.LogWarning("Webhook secret is not configured. Cannot verify signature.");
            return false; // Or true if you want to allow it in dev, but be careful.
        }

        // This is an example. Refer to Chargily's documentation for the exact algorithm.
        var encoding = new UTF8Encoding();
        byte[] keyByte = encoding.GetBytes(_settings.WebhookSecret);
        byte[] messageBytes = encoding.GetBytes(payload);
        using (var hmac = new HMACSHA256(keyByte))
        {
            byte[] hash = hmac.ComputeHash(messageBytes);
            string computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computedSignature.Equals(signatureHeader, StringComparison.OrdinalIgnoreCase);
        }
    }
}