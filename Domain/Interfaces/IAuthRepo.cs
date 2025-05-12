using Domain.Dtos.AuthDtos;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAuthRepo
    {
        Task<int> RegisterUserAsync(RegisterApplicationUserDTO user);
        Task<ApplicationUserDto> FindUserWithEmailAsync(string email);
        Task<ApplicationUserDto> FindUserWithPhoneAsync(string phoneNumber);
        Task<ApplicationUserDto> FindUserWithUsernameAsync(string username);
        Task<ApplicationUserDto> FindUserWithIDAsync(string ID);
        Task<int> LoginUserWithUsernameOrEmailAsync(LoginWithEmailOrUsernameDTO request);
        Task<int> LoginUserWithPhoneNumberAsync(LoginWithPhoneNumberDTO request);
        Task<bool> CheckUserExistAsync(int userID);
    }
}
