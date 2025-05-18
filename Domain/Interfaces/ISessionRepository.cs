using Domain.Dtos.SessionDtos;

namespace Domain.Interfaces
{
    public interface ISessionRepository
    {
        public Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(BookSessionDTO session);
        public Task<GetSessionsByRoleID> GetSessionsByPatientIdAsync(int patientId,string? status);
        public Task<GetSessionsByRoleID> GetSessionsByTherapistIdAsync(int therapistID, string? status);



    }
}
