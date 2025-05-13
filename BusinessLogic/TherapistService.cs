using Domain.Dtos.TherapistDto;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class TherapistService
    {
        private readonly ITherapist _therapistRepo;
        public TherapistService(ITherapist therapistRepo)
        {
            _therapistRepo = therapistRepo;
        }
        public async Task<bool> AddAdditionalInformationTherapist(AdditionalInformationTherapistDto request)
        {
            if (request == null)
            {
                return false;
            }
            return await _therapistRepo.AddAddAdditionalInformationTherapist(request);
        }
    }
}
