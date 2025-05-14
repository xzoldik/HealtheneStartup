using Domain.Dtos.SessionDtos;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SessionRepository:ISessionRepository

    {

        readonly ISessionRepository _sessionRepository;
        public SessionRepository(ISessionRepository sessionRepository) { 
            _sessionRepository = sessionRepository;
        }

        public Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(SessionDTO session)
        {
            throw new NotImplementedException();
        }


    }
}
