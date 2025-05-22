
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.AuthDtos
{
    public class LoginWithEmailOrUsernameDTO
    {
        [Required(ErrorMessage = "Email or Username  is required")]
        public required string LoginIdentifier { get; set; }
        [Required(ErrorMessage = "password is required")]
        [MinLength(6,ErrorMessage ="The password must at least 6 characters")]
        public required string Password { get; set; }


    }
}
