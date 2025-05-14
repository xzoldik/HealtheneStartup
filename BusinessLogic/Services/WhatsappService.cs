using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public static class WhatsappService
    {
        public static async Task<string> SendWhatsappMessage(string message, string phone)
        {
            var url = "https://api.ultramsg.com/instance112104/messages/chat";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", "4kxo59pazi2s8szn");
            request.AddParameter("to", phone);
            request.AddParameter("body", message);

            RestResponse response = await client.ExecuteAsync(request);
            string output = response.Content!;
            return output;

        }
        public static string CreateMessage()
        {
            var otp = new Random().Next(1000, 9999);
            string message = $"Your OTP is: {otp}";
            return message;
        }
    }
}

