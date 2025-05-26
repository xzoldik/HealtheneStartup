using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.AuthDtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public ApplicationUserDto User { get; set; } = new ApplicationUserDto();
        public DateTime ExpiresAt { get; set; }
    }
}
