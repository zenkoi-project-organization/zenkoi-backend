using Net.payOS.Types;
using Zenkoi.BLL.DTOs.PaymentDTOs;
using Zenkoi.BLL.DTOs.PayOSDTOs;
using Zenkoi.BLL.DTOs.VnPayDTOs;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDTO> CreatePaymentLinkAsync(int orderId, string paymentMethod);
        Task<bool> ProcessPaymentCallbackAsync(int orderId, string status);
        Task<string> CreatePayOSPaymentLinkAsync(int userId, PayOSPaymentRequestDTO request);
        Task<string> CreateVnPayPaymentUrlAsync(int userId, VnPayRequestDTO request);
        Task<IEnumerable<PaymentTransaction>> GetPaymentHistoryAsync(int userId);
        Task<PaymentTransaction?> GetPaymentStatusAsync(string orderId);
        Task<PaymentStatusResponseDTO> CheckPaymentStatusByOrderCodeAsync(int orderCode);
    }
}
