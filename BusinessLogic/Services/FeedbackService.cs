using Domain.Dtos.FeedbackDtos;
using Domain.Globals;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class FeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }
        public async Task<ServiceResult<int>> CreateFeedbackAsync(FeedbackDto feedback)
        {
            return await _feedbackRepository.CreateFeedbackAsync(feedback);

        }
        public async Task<ServiceResult<IEnumerable<GetFeedbackDTO>>> GetTherapistFeedbacks(int therapistId, string? sessionType = null)
        {
            return await _feedbackRepository.GetTherapistFeedbacks(therapistId, sessionType);
        }

        public async Task<ServiceResult<GetFeedbackDTO>> GetIndividualSessionFeedback(int sessionId) // Returns single DTO
        {
            return await _feedbackRepository.GetIndividualSessionFeedback(sessionId);
        }

    }
}
