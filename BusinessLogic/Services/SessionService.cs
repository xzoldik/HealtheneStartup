using DataAccess.Repositories;
using Domain.Dtos.SessionDtos;
using Domain.Interfaces;

namespace BusinessLogic.Services
{
    public class SessionService

    {
        private readonly ISessionRepository _SessionRepository;
        public SessionService(ISessionRepository session)
        {
            _SessionRepository = session;
        }

        public async Task<BookSessionResultDTO> BookIndividualSessionAsync(BookSessionDTO session)
        {
            var (sessionId, returnCode, dbErrorMessage) = await _SessionRepository.BookIndividualSessionAsync(session);

            if (sessionId > 0 && returnCode == 0) 
            {
                return new BookSessionResultDTO { Success = true, SessionId = sessionId, Message = "Session booked successfully." };
            }
            else
            {
                string friendlyMessage = "Failed to book session.";
                if (returnCode == -4 || (dbErrorMessage != null && dbErrorMessage.Contains("Therapist is not available")))
                {
                    friendlyMessage = "The therapist is not available at the selected time.";
                }
                else if (returnCode == -5 || (dbErrorMessage != null && dbErrorMessage.Contains("conflicting appointment")))
                {
                    friendlyMessage = "You have a conflicting appointment at the selected time.";
                }
                else if (!string.IsNullOrEmpty(dbErrorMessage))
                {
                    friendlyMessage = $"An error occurred: {dbErrorMessage}"; 
                }
                return new BookSessionResultDTO { Success = false, Message = friendlyMessage, ErrorCode = returnCode.ToString() };
            }
        }
    }
}
