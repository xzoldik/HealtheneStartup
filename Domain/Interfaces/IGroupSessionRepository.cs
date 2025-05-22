using Domain.Dtos.GroupSessionDtos;
using Domain.Dtos.SessionDtos;
using Domain.Globals;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    public interface IGroupSessionRepository
    {
        public Task<(int sessionId, int returnCode, string errorMessage)> BookGroupSessionAsync(BookGroupSessionDto session);
        public Task<int> JoinGroupSessionAsync(int sessionId, int patientId);
        public Task<ServiceResult<GroupSession>> GetGroupSessionByIdAsync(int sessionId, string? status = null);
        public Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByTherapistIdAsync(int therapistId, string? status = null);
        public Task<ServiceResult<List<GroupSession>>> GetGroupSessionsByPatientIdAsync(int patientId, string? status = null);
        public Task<bool> ChangeGroupSessionStatusAsync(int sessionId, string status);
    }






}

