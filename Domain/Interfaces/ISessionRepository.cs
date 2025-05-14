using Domain.Dtos.SessionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISessionRepository
    {
        public Task<(int sessionId, int returnCode, string errorMessage)> BookIndividualSessionAsync(SessionDTO session);



    }
}
