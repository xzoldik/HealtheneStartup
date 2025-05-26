using BusinessLogic.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly TwilioVideoService _twilioVideoService;

        public VideoController(TwilioVideoService twilioVideoService)
        {
            _twilioVideoService = twilioVideoService;
        }

        [HttpGet("token/{sessionId}")]
        [Authorize]
        public IActionResult GetTwilioToken(int sessionId)
        {
            // Get user info from JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
                return Unauthorized("the user id is empty");

            // Use a unique room name per session
            var roomName = $"session-{sessionId}";

            // Use a unique identity for Twilio (e.g., "therapist-5" or "patient-10")
            var identity = $"{userRole.ToLower()}-{userId}";


            var token = _twilioVideoService.GenerateTwilioToken(identity, roomName);

            return Ok(new
            {
                token,
                roomName,
                identity
            });
        }
    }
}