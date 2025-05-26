using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.PaymentDtos
{
    public class CreatePaymentRequest
    {
        [Required(ErrorMessage = "Amount method is required")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Currency method is required")]
        public string Currency { get; set; } = "dzd"; // Default to DZD
        [Required(ErrorMessage = "SessionID method is required")]
        public int SessionID { get; set; } // Assuming this is an ID for the session or user
        [Required(ErrorMessage = "PatientID method is required")]
        public int PatientID { get; set; }
    }
}
