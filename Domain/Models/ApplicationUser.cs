using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ApplicationUser

    {
        public int UserID { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Role { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int CountryID { get; set; }
        public string? ProfilePicture { get; set; }
        public string CreatedAt { get; set; } = DateTime.UtcNow.ToString();
    }
}
