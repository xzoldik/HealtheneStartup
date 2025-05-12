using BusinessLogic.Whatsapp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Healthene.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WhatsappOtp : ControllerBase
    {
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp(string phone)
        {
            string message = WhatsappService.CreateMessage();
            var response = await WhatsappService.SendWhatsappMessage(message, phone);
            return Ok(response);
        }
    }
}
