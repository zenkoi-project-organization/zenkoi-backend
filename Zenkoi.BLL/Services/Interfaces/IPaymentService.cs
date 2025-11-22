using Zenkoi.BLL.DTOs.PaymentDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPaymentService
    {     
        Task<PaymentResponseDTO> CreatePaymentLinkAsync(int orderId, string paymentMethod);
        Task<bool> ProcessPaymentCallbackAsync(int orderId, string status);
    }
}
