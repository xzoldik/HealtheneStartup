// Controllers/PaymentsController.cs
using Domain.Dtos.PaymentDtos;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentsController(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;

    }

    // Endpoint to CREATE a payment
    [HttpPost("initiate")]
    public async Task<IActionResult> InitiatePayment([FromBody] CreatePaymentRequest request)
    {
        var result = await _paymentRepository.CreatePaymentAsync(request);
        if (result == null)
        {
            return BadRequest("Failed to create payment.");
        }
        // Return BOTH the URL and the ID. The client needs the ID to check the status.
        return Ok(new { paymentId = result.Value.paymentId, checkoutUrl = result.Value.checkoutUrl });
    }

    // Endpoint for the client to CHECK the status
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetStatus(int id)
    {
        var status = await _paymentRepository.GetPaymentStatusAsync(id);
        if (status == null)
        {
            return NotFound();
        }
        return Ok(new { status });
    }

    // Endpoint for Chargily to SEND notifications (Webhook)
    [HttpPost("webhook")]
    public async Task<IActionResult> ChargilyWebhook()
    {
        // Chargily sends the payload in the request body and a signature in a header.
        string jsonPayload;
        using (StreamReader reader = new StreamReader(Request.Body))
        {
            jsonPayload = await reader.ReadToEndAsync();
        }

        // Get the signature header (Chargily usually sends it as 'Signature' or 'X-Chargily-Signature')
        // **IMPORTANT**: Verify the exact header name from Chargily's documentation.
        // For Chargily Pay v2, it's typically 'Signature'.
        string? signatureHeader = Request.Headers["Signature"].FirstOrDefault();

        if (string.IsNullOrEmpty(signatureHeader))
        {
            return BadRequest("Missing signature header.");
        }

        try
        {
            await _paymentRepository.ProcessWebhookAsync(jsonPayload, signatureHeader);
            return Ok(); // Respond with 200 OK to Chargily
        }
        catch (System.Exception ex)
        {
            // Return 500 to Chargily so they might retry the webhook
            return StatusCode(500, "Error processing webhook.");
        }
    }

    /// <summary>
    /// Endpoint for handling successful payment redirects from Chargily.
    /// This is the 'success_url' configured in appsettings.json.
    /// </summary>
    /// <param name="paymentId">The local payment ID (can be passed as a query parameter or retrieved from session/cookie if needed).</param>
    /// <returns>A view or message indicating payment success.</returns>
    [HttpGet("/payment/success")] // Note: Using absolute path here as it's a browser redirect
    public async Task<IActionResult> PaymentSuccess([FromQuery] int? paymentId)
    {
        string statusMessage = "Payment successful!";
        if (paymentId.HasValue)
        {
            // Optionally, fetch the payment status from your DB to confirm
            string? status = await _paymentRepository.GetPaymentStatusAsync(paymentId.Value);
            if (status == "Paid")
            {
                statusMessage = $"Payment {paymentId.Value} was successfully processed.";
            }
            else if (status == "Pending")
            {
                statusMessage = $"Payment {paymentId.Value} is pending. Please wait for confirmation.";
            }
            else
            {
                statusMessage = $"Payment {paymentId.Value} status: {status}. Please check your payment history.";
            }
        }
        else
        {
        }

        // In a real application, you'd likely redirect to a frontend page
        // or return a View with a success message.
        return Ok(new { Message = statusMessage });
    }

    /// <summary>
    /// Endpoint for handling cancelled or failed payment redirects from Chargily.
    /// This is the 'cancel_url' configured in appsettings.json.
    /// </summary>
    /// <param name="paymentId">The local payment ID (can be passed as a query parameter or retrieved from session/cookie if needed).</param>
    /// <returns>A view or message indicating payment cancellation/failure.</returns>
    [HttpGet("/payment/cancel")] // Note: Using absolute path here
    public async Task<IActionResult> PaymentCancel([FromQuery] int? paymentId)
    {
        string statusMessage = "Payment cancelled or failed.";
        if (paymentId.HasValue)
        {
            // Optionally, fetch the payment status from your DB to confirm
            string? status = await _paymentRepository.GetPaymentStatusAsync(paymentId.Value);
            if (status == "Failed" || status == "Cancelled")
            {
                statusMessage = $"Payment {paymentId.Value} was cancelled or failed.";
            }
            else if (status == "Pending")
            {
                statusMessage = $"Payment {paymentId.Value} is pending. If you cancelled, it might update soon.";
            }
            else
            {
                statusMessage = $"Payment {paymentId.Value} status: {status}.";
            }
        }
        else
        {
        }

        // In a real application, you'd likely redirect to a frontend page
        // or return a View with a failure message.
        return Ok(new { Message = statusMessage });
    }
}
