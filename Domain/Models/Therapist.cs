using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Therapist

    {
        [Required(ErrorMessage = "Therapist ID is required")]
        public required int TherapistId { get; set; }
        public required int UserID { get; set; }
        public required string? Specialization { get; set; }
        public required string? Bio { get; set; }
        public required int? Rating { get; set; }
        public required string? Diploma { get; set; }
        public required int? YearsOfExperience { get; set; }
        public required string TherapistFirstName { get; set; }
        public required string TherapistLastName { get; set; }
        public required string TherapistEmail { get; set; }
        public required string TherapistPhoneNumber { get; set; }
        public required string? ProfilePicture { get; set; }
        public required string Username { get; set; }
        public required string CountryName { get; set; }
        public required int Age { get; set; }

    }
}
