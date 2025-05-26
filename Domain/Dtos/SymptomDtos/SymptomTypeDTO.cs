using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.SymptomDtos
{
    public class SymptomTypeDTO
    {
        public int SymptomTypeID { get; set; }
        [Required(ErrorMessage = "Severity is required.")]
        [Range(1, 10, ErrorMessage = "Severity must be between 1 and 10.")]
        public decimal Severity { get; set; }
        [Required(ErrorMessage = "therapist ID is required.")]
        public string? Notes { get; set; } = string.Empty;
    }
}

