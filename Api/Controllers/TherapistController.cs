using BusinessLogic.Services;
using Domain.Dtos.TherapistDto;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class therapistController : ControllerBase
    {
        private readonly TherapistService _therapistService;
        public therapistController(TherapistService therapistService)
        {
            _therapistService = therapistService;
        }

        [HttpPost("additional-informations")]
        public async Task<IActionResult> AddAdditionalInformationTherapist([FromBody] AdditionalInformationTherapistDto request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }
            var result = await _therapistService.AddAdditionalInformationTherapist(request);
            if (result)
            {
                return Ok("Additional information added successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while adding additional information");
            }
        }
        [HttpGet("users/{UserID}")]
        public async Task<ActionResult<Therapist>> GetTherapistByUserId( int UserID)
        {
            if (UserID <= 0)
            {
                return BadRequest("Invalid User ID");

            }
            var therapist = await _therapistService.GetTherapistByUserId(UserID);
            if (therapist == null)
            {
                return NotFound("Therapist not found");
            }
            else
            {
                return Ok(therapist);
            }


        }
        [HttpGet("{TherapistID}")]
        public async Task<ActionResult<Therapist>> GetTherapistByTherapistId(int TherapistID)
        {
            if (TherapistID <= 0)
            {
                return BadRequest("Invalid Therapist ID");
            }
            var therapist = await _therapistService.GetTherapistByTherapistID(TherapistID);
            if (therapist == null)
            {
                return NotFound("Therapist not found");
            }
            else
            {
                return Ok(therapist);
            }
        }
        [HttpGet("recommendations/{UserID}")]
        public async Task<ActionResult<IEnumerable<Therapist?>>> GetTop3PsychotherapistMatches(int UserID)
        {
            if(UserID <= 0)
            {
                return BadRequest("Invalid User ID");
            }
            List<Therapist> therapists = await _therapistService.GetTop3PsychotherapistMatches(UserID);

            if(therapists is null)
            {
                return BadRequest("Invalid User ID");

            }
            if (therapists.Count == 0)
            {
                return NotFound("No therapists found");
            }
            else
            {
                return Ok(therapists);
            }

        }
    }
}
