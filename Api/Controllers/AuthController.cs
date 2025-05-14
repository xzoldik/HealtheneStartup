using BusinessLogic.Services;
using Domain.Dtos.AuthDtos;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Healthene.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterApplicationUserDTO user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userID = await _authService.RegisterUserAsync(user);
            if (userID != -1) // If rows were inserted
            {
                return Ok(userID);
            }
            else
            {
                return BadRequest("Registration failed");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUserWithUsernameOrEmailAsync(LoginWithEmailOrUsernameDTO user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userID = await _authService.LoginUserWithUsernameOrEmailAsync(user);
            if (userID != -1) // If rows were inserted
            {
                return Ok(userID);
            }
            else
            {
                return BadRequest("Login failed");
            }
        }

        [HttpPost("Login-phone")]
        public async Task<IActionResult> LoginUserWithPhoneNumberAsync(LoginWithPhoneNumberDTO user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userID = await _authService.LoginUserWithPhoneNumberAsync(user);
            if (userID != -1) // If rows were inserted
            {
                return Ok(userID);
            }
            else
            {
                return Unauthorized(new { message = "Invalid phone or password." });
            }
        }
        [HttpGet("FindUserWithID/{ID}")]
        public async Task<ActionResult<ApplicationUserDto>> FindUserWithIDAsync(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return BadRequest("User ID is required.");
            }
            ApplicationUserDto user = await _authService.FindUserWithIDAsync(ID);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound(new { message = $"User {ID} not found" });
            }

        }
    }
}
