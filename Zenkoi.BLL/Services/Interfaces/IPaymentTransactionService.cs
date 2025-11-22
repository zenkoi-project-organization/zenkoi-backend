using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PaymentTransactionDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPaymentTransactionService
    {
        Task<PaginatedList<PaymentTransactionResponseDTO>> GetAllTransactionsAsync(
            PaymentTransactionFilterDTO filter,
            int pageIndex = 1,
            int pageSize = 10);

        Task<PaginatedList<PaymentTransactionResponseDTO>> GetMyTransactionsAsync(
            int userId,
            PaymentTransactionFilterDTO filter,
            int pageIndex = 1,
            int pageSize = 10);

        Task<PaymentTransactionResponseDTO> GetTransactionByIdAsync(int id);

        Task<PaymentTransactionResponseDTO> GetTransactionByActualOrderIdAsync(int actualOrderId);
    }
}
