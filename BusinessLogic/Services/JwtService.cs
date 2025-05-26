using Domain.Dtos.AuthDtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusinessLogic.Services
{


        public class JwtService // Your JWT Service class
        {
            private readonly IConfiguration _configuration;
            private readonly SymmetricSecurityKey _signingKey; // Store the key once
            private readonly JwtSecurityTokenHandler _tokenHandler; // Store handler once
            private readonly string _issuer;
            private readonly string _audience;
            private readonly double _expireHours;

            public JwtService(IConfiguration configuration)
            {
                _configuration = configuration;

                // Retrieve and store config values once in the constructor
                var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration.");
                _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found in configuration.");
                _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found in configuration.");
                _expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

                _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                _tokenHandler = new JwtSecurityTokenHandler();
            }

            // This method is used to CREATE the JWT after a successful login
            public string GenerateToken(ApplicationUserDto user)
            {
                var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("CountryID", user.CountryID.ToString())
        };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(_expireHours), // Use UtcNow for consistency
                    SigningCredentials = credentials,
                    Issuer = _issuer,
                    Audience = _audience
                };

                var token = _tokenHandler.CreateToken(tokenDescriptor);
                return _tokenHandler.WriteToken(token);
            }
        }
    }

