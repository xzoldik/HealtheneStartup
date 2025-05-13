using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.TherapistDto
{
    public class AdditionalInformationTherapistDto
    {
        [Required(ErrorMessage ="Therapist ID is required")]
        public required int TherapistId { get; set; }
        public required string? Specialization { get; set; }
        public required string? Bio { get; set; }
        public required int? Rating { get; set; }
        public required string? Diploma { get; set; }
        public required int? YearsOfExperience { get; set; }



    }
}
