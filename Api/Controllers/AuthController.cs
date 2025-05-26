using BusinessLogic.Services;
using Domain.Dtos.AuthDtos;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("register")]
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

            try
            {
                var loginResponse = await _authService.RegisterUserWithTokenAsync(user);
                return Ok(new
                {
                    Message = "User registered successfully",
                    Token = loginResponse.Token,
                    User = loginResponse.User,
                    ExpiresAt = loginResponse.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("register-basic")]
        public async Task<IActionResult> RegisterUserBasic([FromBody] RegisterApplicationUserDTO user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userID = await _authService.RegisterUserAsync(user);
                if (userID != -1)
                {
                    return Ok(new { UserId = userID, Message = "User registered successfully. Please login to get your token." });
                }
                else
                {
                    return BadRequest("Registration failed");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
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

            try
            {
                var loginResponse = await _authService.LoginUserWithUsernameOrEmailAsync(user);
                if (loginResponse != null)
                {
                    return Ok(loginResponse);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Login failed", details = ex.Message });
            }
        }

        [HttpPost("login-phone")]
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

            try
            {
                var loginResponse = await _authService.LoginUserWithPhoneNumberAsync(user);
                if (loginResponse != null)
                {
                    return Ok(loginResponse);
                }
                else
                {
                    return Unauthorized(new { message = "Invalid phone or password." });
                }
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Login failed", details = ex.Message });
            }
        }

        [HttpGet("user/{ID}")]
        [Authorize] // Protect this endpoint with JWT
        public async Task<ActionResult<ApplicationUserDto?>> FindUserWithIDAsync(int ID)
        {
            if (ID == 0)
            {
                return BadRequest("User ID is required.");
            }

            ApplicationUserDto? user = await _authService.FindUserWithIDAsync(ID);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound(new { message = $"User {ID} not found" });
            }
        }

        [HttpGet("me")]
        [Authorize] // Get current user info from JWT token
        public async Task<ActionResult<ApplicationUserDto?>> GetCurrentUser()
        {
            int userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            if (userId == 0)
            {
                return Unauthorized();
            }

            ApplicationUserDto? user = await _authService.FindUserWithIDAsync(userId);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound(new { message = "User not found" });
            }
        }
    }
}