using BusinessLogic;
using Domain.Dtos.TherapistDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TherapistController : ControllerBase
    {
        private readonly TherapistService _therapistService;
        public TherapistController(TherapistService therapistService)
        {
            _therapistService = therapistService;
        }

        [HttpPost("AddAdditionalInformationTherapist/")]
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
    }
}
