using DataAccess;
using Domain.Dtos.AuthDtos;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class AuthService
    {
        private readonly IAuthRepo _authRepo;
        public AuthService(IAuthRepo authRepo)
        {
            _authRepo = authRepo;  }
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
        public async Task<int> LoginUserWithUsernameOrEmailAsync(LoginWithEmailOrUsernameDTO user)
        {
            int UserID = await _authRepo.LoginUserWithUsernameOrEmailAsync(user);
            if (UserID != -1)
            {
                return UserID;
            }
            else
            {
                throw new Exception("Registration failed");
            }
        }

        public async Task<int> LoginUserWithPhoneNumberAsync(LoginWithPhoneNumberDTO user)
        {
            int UserID = await _authRepo.LoginUserWithPhoneNumberAsync(user);
            if(UserID != -1)
            {
                return UserID;
            }
            else
            {
                return -1;
            }

        }

        public async Task<ApplicationUserDto> FindUserWithIDAsync(string ID)
        {
            ApplicationUserDto user = await _authRepo.FindUserWithIDAsync(ID);
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
