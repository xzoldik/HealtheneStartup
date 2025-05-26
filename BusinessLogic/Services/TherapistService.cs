using Domain.Dtos.TherapistDto;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class TherapistService
    {
        private readonly ITherapistRepository _therapistRepo;
        public TherapistService(ITherapistRepository therapistRepo)
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

        public async Task<Therapist> GetTherapistByUserId(int UserID)
        {
            if (UserID <= 0)
            {
                throw new ArgumentException("Invalid User ID");
            }
            Therapist Therapist = await _therapistRepo.GetTherapistByUserId(UserID);
            if (Therapist == null)
            {
                return null;
            }
            else
            {
                return Therapist;
            }
        }

        public async Task<Therapist> GetTherapistByTherapistID(int TherapistID)
        {
            if (TherapistID <= 0)
            {
                throw new ArgumentException("Invalid User ID");
            }
            Therapist Therapist = await _therapistRepo.GetTherapistByTherapistId(TherapistID);
            if (Therapist == null)
            {
                return null;
            }
            else
            {
                return Therapist;
            }
        }
        public async Task<List<Therapist>> GetTop3PsychotherapistMatches(int UserID)
        {
            if (UserID <= 0)
            {
                throw new ArgumentException("Invalid User ID");

            }
            List<Therapist> therapists = await _therapistRepo.GetTop3PsychotherapistMatches(UserID);
            if (therapists.Count == 0)
            {
                return null;
            }
            else
            {
                return therapists;
            }
        }
    }
}
