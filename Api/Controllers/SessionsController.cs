using BusinessLogic.Services;
using Domain.Dtos.GroupSessionDtos;
using Domain.Dtos.SessionDtos;
using Domain.Globals;
using Domain.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly SessionService _sessionService;
        private readonly GroupSessionService _groupSessionService;

        public SessionsController(SessionService sessionService, GroupSessionService groupSessionService)
        {
            _sessionService = sessionService;
            _groupSessionService = groupSessionService;
        }
        [HttpPost("book")]
        public async Task<ActionResult<BookSessionResultDTO>> BookIndividualSessionAsync(BookSessionDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _sessionService.BookIndividualSessionAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.ErrorCode == -4 || result.ErrorCode == -5)
                {
                    return Conflict(new { message = result.Message });
                }
                return BadRequest(new { message = result.Message });
            }
        }
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<GetSessionsByRoleID>> GetSessionsByPatientIdAsync(int patientId)
        {
            if (patientId <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }
            GetSessionsByRoleID result = await _sessionService.GetSessionsByPatientIdAsync(patientId);
            if (result.returnCode == 0)
            {
                return Ok(result);
            }
            else if (result.returnCode == -1)
            {
                return NotFound(new { message = result.returnMessage });
            }
            else if (result.returnCode == -2)
            {
                return NotFound(new { message = result.returnMessage });
            }

            else
            {
                return NotFound(new { message = result.returnMessage });
            }
        }
        [HttpGet("patient/{patientId}/{status}")]
        public async Task<ActionResult<GetSessionsByRoleID>> GetSessionsByPatientIdFilteredByStatusAsync(int patientId, string status)
        {
            if (patientId <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }
            GetSessionsByRoleID result = await _sessionService.GetSessionsByPatientIdFilteredByStatusAsync(patientId, status);
            if (result.returnCode == 0)
            {
                return Ok(result);
            }
            else if (result.returnCode == -1)
            {
                return NotFound(new { message = result.returnMessage });
            }
            else if (result.returnCode == -2)
            {
                return NotFound(new { message = result.returnMessage });
            }

            else
            {
                return NotFound(new { message = result.returnMessage });
            }
        }
        [HttpGet("therapist/{therapistID}")]
        public async Task<ActionResult<GetSessionsByRoleID>> GetSessionsByTherapistIdAsync(int therapistID)
        {
            if (therapistID <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }
            GetSessionsByRoleID result = await _sessionService.GetSessionsByTherapistIdAsync(therapistID);
            if (result.returnCode == 0)
            {
                return Ok(result);
            }
            else if (result.returnCode == -1)
            {
                return NotFound(new { message = result.returnMessage });
            }
            else if (result.returnCode == -2)
            {
                return NotFound(new { message = result.returnMessage });
            }

            else
            {
                return NotFound(new { message = result.returnMessage });
            }
        }

        [HttpGet("therapist/{therapistID}/{status}")]
        public async Task<ActionResult<GetSessionsByRoleID>> GetSessionsByTherapistIdFilteredByStatusAsync(int therapistID, string status)
        {
            if (therapistID <= 0)
            {
                return BadRequest("Invalid therapist ID.");
            }
            GetSessionsByRoleID result = await _sessionService.GetSessionsByTherapistIdFilteredByStatusAsync(therapistID, status);
            if (result.returnCode == 0)
            {
                return Ok(result);
            }
            else if (result.returnCode == -1)
            {
                return NotFound(new { message = result.returnMessage });
            }
            else if (result.returnCode == -2)
            {
                return NotFound(new { message = result.returnMessage });
            }

            else
            {
                return NotFound(new { message = result.returnMessage });
            }
        }
        [HttpPut("{sessionId}")]
        public async Task<ActionResult<ServiceResult<bool>>> ChangeSessionStatusAsync(int sessionId, [FromQuery]string status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _sessionService.ChangeIndividualSessionStatusAsync(sessionId, status);
            if (result.IsSuccess)
            {
                return StatusCode(201, new { message = "Session status updated successfully." });
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 2:
                        return NotFound(new { message = result.ErrorMessage });
                    case 3:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 4:
                        return Ok(new { message = result.ErrorMessage });
                    case 5:
                        return StatusCode(500, new { message = result.ErrorMessage }); // Message will be "The session status is already set to the new value, or no changes were required."
                    case 99:
                        return StatusCode(500, new { message = $"An unexpected error occurred: {result.ErrorMessage}", ErrorCode = result.ErrorCode });
                    default:
                        return StatusCode(500, new { message = $"An unexpected error occurred: {result.ErrorMessage}", ErrorCode = result.ErrorCode });
                }
            }
        }
        [HttpPost("book-group")]
        public async Task<ActionResult<BookSessionResultDTO>> BookGroupSessionAsync(BookGroupSessionDto sessionRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            BookSessionResultDTO bookSessionResultDTO = await _groupSessionService.BookGroupSessionAsync(sessionRequest);

            if (bookSessionResultDTO.Success && bookSessionResultDTO.SessionId != -1)
            {
                return Ok(bookSessionResultDTO.SessionId);
            }
            else if (bookSessionResultDTO.ErrorCode == -1)
            {

                return BadRequest(bookSessionResultDTO.Message);
            }
            else if (bookSessionResultDTO.ErrorCode == -2)
            {
                return NotFound(bookSessionResultDTO.Message);
            }
            else if (bookSessionResultDTO.ErrorCode == -4)
            {
                return Conflict(bookSessionResultDTO.Message);
            }
            else
            {
                return StatusCode(500, new { message = bookSessionResultDTO.Message });
            }


        }

        [HttpPost("/{patientID}/join-group/{sessionID}")]
        public async Task<ActionResult<JoinGroupSessionDto>> JoinGroupSession(int patientID, int sessionID)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (patientID <= 0 || sessionID <= 0)
            {
                return BadRequest("Invalid Patient ID or Session ID.");
            }
            JoinGroupSessionDto result = await _groupSessionService.JoinGroupSession(patientID, sessionID);
            if (result.returncode == 0)
            {
                return Ok(result.message);
            }
            else if (result.returncode == -1)
            {
                return BadRequest(new { message = result.message });
            }
            else if (result.returncode == -2 || result.returncode == -3)
            {
                return NotFound(new { message = result.message });
            }
            else if (result.returncode == -4 || result.returncode == -5 || result.returncode == -6 || result.returncode == -7)
            {
                return Conflict(new { message = result.message });
            }
            else
            {
                return StatusCode(500, new { message = result.message });
            }
        }
        [HttpGet("group/{sessionId}")]
        public async Task<ActionResult<GroupSession>> GetGroupSessionByIdAsync(int sessionId, string? status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Basic validation for input ID
            if (sessionId <= 0)
            {
                return BadRequest(new { message = "Invalid session ID." });
            }

            // Call the service layer, which now returns a ServiceResult
            ServiceResult<GroupSession> result = await _groupSessionService.GetGroupSessionByIdAsync(sessionId, status);

            if (result.IsSuccess)
            {
                // If successful, return the data with OK (200) status
                return Ok(result.Data);
            }
            else
            {
                // Handle different error codes from the service/repository
                switch (result.ErrorCode)
                {
                    case 1: // Invalid Session ID (from service validation)
                        return BadRequest(new { message = result.ErrorMessage });
                    case 2: // Group session not found (from SP or repository)
                        return NotFound(new { message = result.ErrorMessage });
                    case 3: // Group session not found (from SP or repository)
                        return NotFound(new { message = result.ErrorMessage });
                    case 99: // General database error
                        return StatusCode(500, new { message = $"Database error: {result.ErrorMessage}" });
                    case 100: // Unexpected internal error
                        return StatusCode(500, new { message = $"An unexpected error occurred: {result.ErrorMessage}" });
                    default: // Catch-all for any other unhandled errors
                        return StatusCode(500, new { message = "An unknown error occurred while retrieving the group session." });
                }
            }
        }
        [HttpGet("group/patient/{patientId}")]
        public async Task<ActionResult<List<GroupSession>>> GetGroupSessionsByPatientIdAsync(int patientId, [FromQuery] string? status = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result = await _groupSessionService.GetGroupSessionsByPatientIdAsync(patientId, status);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 2:
                        return NotFound(new { message = result.ErrorMessage });
                    case 3:
                        return NotFound(new { message = result.ErrorMessage });
                    case 4:
                        return NotFound(new { message = result.ErrorMessage });
                    case 99:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });

                    default:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });
                }
            }
        }
        [HttpGet("group/therapist/{therapistId}")]
        public async Task<ActionResult<List<GroupSession>>> GetGroupSessionsByTherapistIdAsync(int therapistId, [FromQuery] string? status = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _groupSessionService.GetGroupSessionsByTherapistIdAsync(therapistId, status);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 2:
                        return NotFound(new { message = result.ErrorMessage });
                    case 3:
                        return NotFound(new { message = result.ErrorMessage });
                    case 4:
                        return NotFound(new { message = result.ErrorMessage });
                    case 99:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });
                    default:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });
                }
            }
        }
        [HttpPut("group/{sessionId}")]
        public async Task<ActionResult<ServiceResult<bool>>> ChangeGroupSessionStatusAsync(int sessionId, string status)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _groupSessionService.ChangeGroupSessionStatusAsync(sessionId, status);
            if (result.IsSuccess)
            {
                return StatusCode(201, new { message = "Group session status updated successfully." });
            }
            else
            {
                switch (result.ErrorCode)
                {
                    case 1:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 2:
                        return NotFound(new { message = result.ErrorMessage });
                    case 3:
                        return BadRequest(new { message = result.ErrorMessage });
                    case 4:
                        return Ok(new { message = result.ErrorMessage });
                    case 5:
                        return StatusCode(500,new { message = result.ErrorMessage }); // Message will be "The group session status is already set to the new value, or no changes were required."
                    case 99:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });
                    default:
                        return StatusCode(500, new
                        {
                            message = $"An unexpected error occurred: {result.ErrorMessage}",
                            ErrorCode = result.ErrorCode
                        });
                }
            }
        }


    }
}
