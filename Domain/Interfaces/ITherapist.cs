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
        Task<bool> AddAddAdditionalInformationTherapist(AdditionalInformationTherapistDto request);


        Task<List<Therapist>> GetTop3PsychotherapistMatches(int PatientID);

    }
}
