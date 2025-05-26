using Domain.Dtos.SymptomDtos;
using Domain.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISymptomRepository
    {
        public Task<ServiceResult<int>> AddSymptomAsync(AddSymptomDto symptoms);
        public Task<ServiceResult<IEnumerable<SymptomHistoricDto>>> GetPatientSymptomsAsync(int patientId);
    }
}
