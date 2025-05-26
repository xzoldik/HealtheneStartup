using Domain.Dtos.MatchingSystemDtos;
using Domain.Dtos.QuestionnaireDtos;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IQuestionnaireRepository
    {
        Task<List<Questionnaire>> GetAllQuestionnairesAsync();
        Task<Questionnaire?> GetQuestionnaireByIdAsync(int id);
        Task<Questionnaire> CreateQuestionnaireAsync(Questionnaire questionnaire);
        Task<Questionnaire?> UpdateQuestionnaireAsync(Questionnaire questionnaire);
        Task<bool> SavePatientPreferencesAsync(AddPreferencesPatientDto preferences);
        Task<bool> SaveTherapistPreferencesAsync(AddPreferencesTherapistDto preferences);
        Task<List<TherapistDto>> MatchTherapistsWithPatient(int patientId);
        Task<bool> DeleteQuestionnaireAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}