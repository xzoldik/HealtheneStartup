using DataAccess.Repositories;
using Domain.Dtos.GroupSessionDtos;
using Domain.Dtos.SessionDtos;
using Domain.Globals;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class GroupSessionService
    {

        private readonly IGroupSessionRepository _groupSessionRepository;
        public GroupSessionService(IGroupSessionRepository groupSessionRepository)
        {
            _groupSessionRepository = groupSessionRepository;
        }
        public async Task<BookSessionResultDTO> BookGroupSessionAsync(BookGroupSessionDto sessionRequest)
        {

            var (sessionId, returnCode, errorMessage) = await _groupSessionRepository.BookGroupSessionAsync(sessionRequest);
            switch (returnCode)
            {
                case 0:

                    errorMessage = "Group session booked successfully.";
                    break;
                case -1:
                    errorMessage = "Required parameters cannot be null.";
                    break;
                case -2:
                    errorMessage = "Therapist not found for this ID.";
                    break;
                case -4:
                    errorMessage = "Therapist is not available at the selected time due to an existing session.";
                    break;
                case -99:
                    errorMessage = "An unexpected error occurred during booking (SP internal error).";
                    break;
                default:
                    errorMessage = $"An unhandled error code returned by the stored procedure: {returnCode}.";
                    break;
            }

            if (returnCode == 0)
            {
                return new BookSessionResultDTO { Success = true, SessionId = sessionId, ErrorCode = returnCode, Message = errorMessage };
            }
            else
            {
                return new BookSessionResultDTO { Success = false, SessionId = sessionId, ErrorCode = returnCode, Message = errorMessage };
            }

        }
        public async Task<JoinGroupSessionDto> JoinGroupSession(int patientID, int sessionID)
        {
            var returnCode = await _groupSessionRepository.JoinGroupSessionAsync(patientID, sessionID);
            string message = string.Empty;
            switch (returnCode)
            {
                case 0:
                    message = "Patient joined the group session successfully.";
                    break;
                case -1:
                    message = "Required parameters cannot be null.";
                    break;
                case -2:
                    message = "Patient not found for this ID.";
                    break;
                case -3:
                    message = "Group session not found for this ID.";
                    break;
                case -4:
                    message = "Group session cannot be joined";
                    break;
                case -5:
                    message = "Patient is already registered for this group session.";
                    break;
                case -6:
                    message = "Group session is already full.";
                    break;
                case -7:
                    message = "Patient is already booked for another session that overlaps with this group session.";
                    break;
                case -99:
                    message = "An unexpected error occurred during booking (SP internal error).";
                    break;
                default:
                    message = $"An unhandled error code returned by the stored procedure: {returnCode}.";
                    break;
            }
            return new JoinGroupSessionDto { success = returnCode == 0, returncode = returnCode, message = message };
        }
        public async Task<ServiceResult<GroupSession>> GetGroupSessionByIdAsync(int sessionId, string? status = null)
        {
            // Input validation at service layer
            if (sessionId <= 0)
            {
                // Fixed: Actually return the failure result (was missing 'return')
                return ServiceResult<GroupSession>.Failure("Invalid session ID.", -1);
            }

            try
            {
                // Call repository method and return its result
                return await _groupSessionRepository.GetGroupSessionByIdAsync(sessionId, status);
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions at service layer
                // Log the exception here if you have logging configured
                return ServiceResult<GroupSession>.Failure($"Service error: {ex.Message}", -500);
            }
        }
        public async Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByPatientIdAsync(int patientId, string? status = null)
        {
            return await _groupSessionRepository.GetGroupSessionsByPatientIdAsync(patientId, status);

        }
        public async Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByTherapistIdAsync(int therapistId, string? status = null)
        {
            return await _groupSessionRepository.GetGroupSessionsByTherapistIdAsync(therapistId, status);
        }
        public async Task<ServiceResult<bool>> ChangeGroupSessionStatusAsync(int sessionId, string status)
        {
            return await _groupSessionRepository.ChangeGroupSessionStatusAsync(sessionId, status);
        }
    }
}
