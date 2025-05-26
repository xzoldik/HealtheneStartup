using Domain.Dtos.FeedbackDtos;
using Domain.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFeedbackRepository
    {
        public Task<ServiceResult<int>> CreateFeedbackAsync(FeedbackDto feedback);
        public Task<ServiceResult<IEnumerable<GetFeedbackDTO>>> GetTherapistFeedbacks(int TherapistID, string? SessionType);
        public Task<ServiceResult<GetFeedbackDTO>> GetIndividualSessionFeedback(int SessionID);

    }
}
