using Domain.Dtos.SessionDtos;

namespace Domain.Interfaces
{
    public interface ISessionRepository
    {
        public Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(BookSessionDTO session);

        public Task<IEnumerable<SessionDataModel>> GetSessionsByPatientIdAsync(int patientId);


    }
}
