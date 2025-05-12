using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.AuthDtos
{
    public class RegisterApplicationUserDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(patient|therapist|listener)$", ErrorMessage ="Role must be either patient or therapist or listener")]
        public required string Role { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Country ID is required")]
        [Range(2, 196, ErrorMessage = "Country ID must be a positive number")]
        public required int CountryID { get; set; }

        public string? ProfilePicture { get; set; }
    }
}
