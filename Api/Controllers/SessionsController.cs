using BusinessLogic.Services;
using Domain.Dtos.SessionDtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionsController(SessionService sessionService) {
            _sessionService = sessionService;
        }
        [HttpPost("book")]
        public async Task<ActionResult<BookSessionResultDTO>> BookIndividualSessionAsync(BookSessionDTO request)
        {
            if(!ModelState.IsValid)
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
                if (result.ErrorCode == "-4" || result.ErrorCode == "-5")
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
        public async Task<ActionResult<GetSessionsByRoleID>> GetSessionsByPatientIdFilteredByStatusAsync(int patientId,string status)
        {
            if (patientId <= 0)
            {
                return BadRequest("Invalid Patient ID.");
            }
            GetSessionsByRoleID result = await _sessionService.GetSessionsByPatientIdFilteredByStatusAsync(patientId,status);
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
            GetSessionsByRoleID result = await _sessionService.GetSessionsByTherapistIdFilteredByStatusAsync(therapistID,status);
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





    }
}
