using Domain.Dtos.PaymentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public  interface IPaymentRepository
    {
        Task<(int paymentId, string? checkoutUrl)?> CreatePaymentAsync(CreatePaymentRequest request);
        Task<string?> GetPaymentStatusAsync(int paymentId);
        Task ProcessWebhookAsync(string jsonPayload, string signatureHeader);
    }
}

