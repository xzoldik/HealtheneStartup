using BusinessLogic.Services;
using Domain.Dtos.SymptomDtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SymptomController : ControllerBase
    {
        private readonly SymptomService _symptomService;

        public SymptomController(SymptomService symptomService)
        {
            _symptomService = symptomService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSymptom([FromBody] AddSymptomDto symptoms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (symptoms == null)
            {
                return BadRequest("Symptoms data cannot be null.");
            }

            var result = await _symptomService.AddSymptomAsync(symptoms);

            if (result.IsSuccess)
            {
                return StatusCode(201, new
                {
                    SymptomId = result.Data,
                    Message = "Symptoms added successfully."
                });
            }
            else
            {
                // You can expand this switch for more error codes as needed
                switch (result.ErrorCode)
                {
                    case 0:
                        return BadRequest(result.ErrorMessage);
                    case 1:
                        return NotFound(result.ErrorMessage);
                    case 2:
                        return BadRequest(result.ErrorMessage);
                    default:
                        return StatusCode(500, new { message = "An unexpected error occurred.", details = result.ErrorMessage });
                }
            }
        }
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientSymptoms( int patientId)
        {
            if (patientId <= 0)
            {
                return BadRequest("Invalid patient ID.");
            }
            var result = await _symptomService.GetPatientSymptomsAsync(patientId);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                // You can expand this switch for more error codes as needed
                switch (result.ErrorCode)
                {

                    case 1:
                        return NotFound(result.ErrorMessage);
                    case 2:
                        return NotFound(result.ErrorMessage);
                    case 3:
                        return BadRequest(result.ErrorMessage);
                    case 99:
                        return StatusCode(500, new
                        {
                            message = result.ErrorMessage
                        });
                    default:
                        return StatusCode(500, new { message = "An unexpected error occurred.", details = result.ErrorMessage });
                }
            }
        }
    }
}