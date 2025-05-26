using Domain.Dtos.SymptomDtos;
using Domain.Globals;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class SymptomService
    {
        private readonly ISymptomRepository _symptomRepository;
        public SymptomService(ISymptomRepository symptomRepository)
        {
            _symptomRepository = symptomRepository;
        }
        public async Task<ServiceResult<int>> AddSymptomAsync(AddSymptomDto symptoms)
        {
            if (symptoms == null)
            {
                return ServiceResult<int>.Failure("Symptoms data cannot be null.", 0);
            }
            if (symptoms.SymptomTypes == null || !symptoms.SymptomTypes.Any())
            {
                return ServiceResult<int>.Failure("At least one symptom type is required.", 0);
            }

            return await _symptomRepository.AddSymptomAsync(symptoms);
          }
        public async Task<ServiceResult<IEnumerable<SymptomHistoricDto>>> GetPatientSymptomsAsync(int patientId)
        {
            if (patientId <= 0)
            {
                return ServiceResult<IEnumerable<SymptomHistoricDto>>.Failure("Invalid patient ID.", 0);
            }
            return await _symptomRepository.GetPatientSymptomsAsync(patientId);
        }
    }
}
