using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ChargilySettings
    {
        public string ApiUrl { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string success_url { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
    }
}