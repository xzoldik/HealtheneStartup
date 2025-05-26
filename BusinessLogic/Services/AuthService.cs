using DataAccess;
using Domain.Dtos.AuthDtos;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepo, JwtService jwtService, IConfiguration configuration)
        {
            _authRepo = authRepo;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<int> RegisterUserAsync(RegisterApplicationUserDTO user)
        {
            int UserID = await _authRepo.RegisterUserAsync(user);
            if (UserID != -1)
            {
                return UserID;
            }
            else
            {
                throw new Exception("Registration failed");
            }
        }

        public async Task<LoginResponseDto> RegisterUserWithTokenAsync(RegisterApplicationUserDTO user)
        {
            int UserID = await _authRepo.RegisterUserAsync(user);
            if (UserID != -1)
            {
                // Get the newly created user details
                var userDetails = await _authRepo.FindUserWithIDAsync(UserID);
                if (userDetails != null)
                {
                    // Generate JWT token for the new user
                    var token = _jwtService.GenerateToken(userDetails);
                    var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

                    return new LoginResponseDto
                    {
                        Token = token,
                        User = userDetails,
                        ExpiresAt = DateTime.UtcNow.AddHours(expireHours)
                    };
                }
                else
                {
                    throw new Exception("Failed to retrieve user details after registration");
                }
            }
            else
            {
                throw new Exception("Registration failed");
            }
        }

        public async Task<LoginResponseDto?> LoginUserWithUsernameOrEmailAsync(LoginWithEmailOrUsernameDTO user)
        {
            int UserID = await _authRepo.LoginUserWithUsernameOrEmailAsync(user);
            if (UserID != -1)
            {
                // Get user details
                var userDetails = await _authRepo.FindUserWithIDAsync(UserID);
                if (userDetails != null)
                {
                    // Generate JWT token
                    var token = _jwtService.GenerateToken(userDetails);
                    var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

                    return new LoginResponseDto
                    {
                        Token = token,
                        User = userDetails,
                        ExpiresAt = DateTime.UtcNow.AddHours(expireHours)
                    };
                }
            }

            throw new Exception("Login failed");
        }

        public async Task<LoginResponseDto?> LoginUserWithPhoneNumberAsync(LoginWithPhoneNumberDTO user)
        {
            int UserID = await _authRepo.LoginUserWithPhoneNumberAsync(user);
            if (UserID != -1)
            {
                // Get user details
                var userDetails = await _authRepo.FindUserWithIDAsync(UserID);
                if (userDetails != null)
                {
                    // Generate JWT token
                    var token = _jwtService.GenerateToken(userDetails);
                    var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);

                    return new LoginResponseDto
                    {
                        Token = token,
                        User = userDetails,
                        ExpiresAt = DateTime.UtcNow.AddHours(expireHours)
                    };
                }
            }

            return null;
        }

        public async Task<ApplicationUserDto?> FindUserWithIDAsync(int ID)
        {
            ApplicationUserDto? user = await _authRepo.FindUserWithIDAsync(ID);
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}