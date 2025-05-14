using Domain.Dtos.QuestionnaireDtos;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class QuestionnaireService
    {
        private readonly IQuestionnaireRepo _questionnaireRepo;
        public QuestionnaireService(IQuestionnaireRepo questionnaireRepo)
        {
            _questionnaireRepo = questionnaireRepo;
        }

        public async Task<Questionnaire?> GetQuestionnaireByIdAsync(int id)
        {
            Questionnaire? questionnaire = await _questionnaireRepo.GetQuestionnaireByIdAsync(id);
            if (questionnaire != null)
            {
                return questionnaire;
            }
            else
            {
                return null;
            }
        }
        public async Task<bool> SavePreferencesPatientAsync(PreferencesPatientDto preferences)
        {
            if (preferences == null)
            {
                return false;
            }
            var result = await _questionnaireRepo.SavePatientPreferencesAsync(preferences);
            return result;
        }

        public async Task<bool> SavePreferencesTherapistAsync(PreferencesTherapistDto preferences)
        {
            try
            {
                var result = await _questionnaireRepo.SaveTherapistPreferencesAsync(preferences);
                return result; // Return true if successful
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in SavePreferencesTherapistAsync: {ex.Message}");
                throw; // Re-throw the exception to propagate it
            }


        }
    }
}
