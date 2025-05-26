using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.SymptomDtos
{
    public class AddSymptomDto
    {
        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }
        [Required(ErrorMessage = "Symptom type ID is required.")]
        public int TherapistID { get; set; }
        [Required(ErrorMessage = "Symptom field  is required.")]
        public List<SymptomTypeDTO> SymptomTypes { get; set; } = new List<SymptomTypeDTO>();


    }
}
