using BusinessLogic.Services;
using Domain.Dtos.SessionDtos;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly SessionService _sessionService;

        public SessionsController(SessionService sessionService) {
            _sessionService = sessionService;
        }
        [HttpPost("book")]
        public async Task<ActionResult<BookSessionResultDTO>> BookSessionResultDTOBookIndividualSessionAsync(BookSessionDTO request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _sessionService.BookIndividualSessionAsync(request);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                if (result.ErrorCode == "-4" || result.ErrorCode == "-5")
                {
                    return Conflict(new { message = result.Message });
                }
                return BadRequest(new { message = result.Message });
            }
        }
    }
}
