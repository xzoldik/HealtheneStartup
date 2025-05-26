using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain.Dtos.FeedbackDtos; // Make sure this namespace is correct for your FeedbackDto
using Domain.Globals; // Make sure this namespace is correct for your ServiceResult
using BusinessLogic.Services; // Make sure this namespace is correct for your FeedbackService
using System.Threading.Tasks; // Required for async/await

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("CreateFeedback")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackDto feedback)
        {
            // Initial model validation handled by ASP.NET Core
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns a detailed object with validation errors
            }

            if (feedback == null)
            {
                // This check catches an entirely empty/null request body
                return BadRequest("Feedback data cannot be null.");
            }

            var result = await _feedbackService.CreateFeedbackAsync(feedback);

            if (result.IsSuccess)
            {

                return StatusCode(StatusCodes.Status201Created, new
                {
                    feedbackId = result.Data,
                    message = "Feedback added successfully."
                });
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1:
                        return BadRequest(result.ErrorMessage);
                    case 2:
                        return NotFound(result.ErrorMessage);

                    default:

                        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected server error occurred. Please try again later.");
                }
            }
        }
        [HttpGet("Therapist/{therapistId}")]
        public async Task<IActionResult> GetTherapistFeedbacks(
           int therapistId,
           [FromQuery] string? sessionType = null) // Accept sessionType as a query parameter
        {
            if (therapistId <= 0)
            {
                return BadRequest("Invalid Therapist ID. Must be a positive integer.");
            }

            // Optional: Basic validation for sessionType in the controller if you want to catch it early
            if (sessionType != null && !string.Equals(sessionType, "Individual", StringComparison.OrdinalIgnoreCase) && !string.Equals(sessionType, "Group", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid sessionType. Must be 'Individual' or 'Group'.");
            }

            var result = await _feedbackService.GetTherapistFeedbacks(therapistId, sessionType); // Pass the sessionType

            if (result.IsSuccess)
            {
                if (result.Data == null || !result.Data.Any())
                {
                    return NotFound($"No feedback found for Therapist ID: {therapistId}" +
                                    (sessionType != null ? $" with Session Type: {sessionType}." : "."));
                }
                return Ok(result.Data);
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1: // RAISERROR('Invalid TherapistID. Therapist does not exist.', 16, 1);
                        return NotFound(result.ErrorMessage);
                    case 2: // RAISERROR('Session type must be either ''Individual'' or ''Group'' if provided.', 16, 10);
                        return BadRequest(result.ErrorMessage); // This is a client-side validation error
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving feedbacks.");
                }
            }
        }

        [HttpGet("IndividualSession/{sessionId}")]
        public async Task<IActionResult> GetIndividualSessionFeedback(int sessionId)
        {
            if (sessionId <= 0)
            {
                return BadRequest("Invalid Session ID. Must be a positive integer.");
            }

            var result = await _feedbackService.GetIndividualSessionFeedback(sessionId); // Result is now single DTO

            if (result.IsSuccess)
            {
                // Data will be null if no feedback was found in the repository.
                if (result.Data == null)
                {
                    return NotFound($"No feedback found for Individual Session ID: {sessionId}.");
                }
                return Ok(result.Data); // Return the single feedback DTO
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1: // RAISERROR('Invalid SessionID. Individual session does not exist.', 16, 1);
                        return NotFound(result.ErrorMessage);
                    default:
                        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving individual session feedback.");
                }
            }
        }



    }
}