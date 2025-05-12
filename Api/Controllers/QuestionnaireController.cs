using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataAccess;
using Domain.Models;
using Domain.Interfaces;
using Domain.Dtos.QuestionnaireDtos;
using BusinessLogic;


namespace QuestionnaireApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class QuestionnaireController : ControllerBase
    {
        private readonly QuestionnaireService _questionnaireService;

        public QuestionnaireController(QuestionnaireService questionnaireService)
        {
            _questionnaireService = questionnaireService;

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Questionnaire>> GetQuestionnaireByIdAsync(int id)
        {
            var questionnaire = await _questionnaireService.GetQuestionnaireByIdAsync(id);
            if (questionnaire == null)
            {
                return NotFound();
            }
            return Ok(questionnaire);
        }


        [HttpPost("PatientPreferences")]
        public async Task<IActionResult> SubmitPatientPreferences([FromBody] PreferencesPatientDto preferences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var preferenceSaved = await _questionnaireService.SavePreferencesPatientAsync(preferences);
                if (preferenceSaved)
                {
                    return Ok(new { message = "Preferences saved successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "No rows were affected. Preferences were not saved." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error in SubmitPreferences: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while saving preferences.", error = ex.Message });
            }
        }
        [HttpPost("TherapistPreferences")]
        public async Task<IActionResult> SubmitTherapistPreferences([FromBody] PreferencesTherapistDto preferences)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var preferenceSaved = await _questionnaireService.SavePreferencesTherapistAsync(preferences);
                if (preferenceSaved)
                {
                    return Ok(new { message = "Preferences saved successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "No rows were affected. Preferences were not saved." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error in SubmitPreferences: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while saving preferences.", error = ex.Message });
            }
        }

    }
}

