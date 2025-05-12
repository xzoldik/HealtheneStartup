using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.AuthDtos
{
    public class LoginWithPhoneNumberDTO
    {
        [Required(ErrorMessage ="The phone number is required")]
        [Phone(ErrorMessage ="The phone must have a correct format")]
        public required string Phone { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public required string Password { get; set; }
    }
}
