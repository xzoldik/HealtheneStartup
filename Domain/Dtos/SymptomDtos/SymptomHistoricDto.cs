using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.SymptomDtos
{
    public class SymptomHistoricDto
    {
        public int SymptomID { get; set; }
        public string SymptomTypeDescription { get; set; } = string.Empty;  
        public string SymptomCategory { get; set; } = string.Empty;
        public decimal Severity { get; set; }
        public DateTime RecordedAt { get; set; }
        public int TherapistID { get; set; }
        public string TherapistFirstName { get; set; } = string.Empty;
        public string TherapistLastName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
