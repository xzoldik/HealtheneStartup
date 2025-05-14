using Domain.Dtos.TherapistDto;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITherapist
    {

        public Task<bool> AddAddAdditionalInformationTherapist(AdditionalInformationTherapistDto request);
        public Task<Therapist> GetTherapistByTherapistId(int TherapistID);
        public Task<Therapist> GetTherapistByUserId(int UserID);

        public Task<List<Therapist>> GetTop3PsychotherapistMatches(int UserID);

    }
}
