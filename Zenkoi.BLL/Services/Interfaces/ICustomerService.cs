using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateCustomerProfileAsync(int userId);
        Task<CustomerResponseDTO> GetCustomerByUserIdAsync(int userId);
        Task<IEnumerable<CustomerResponseDTO>> GetAllCustomersAsync(QueryOptions<Customer>? queryOptions = null);
        Task<CustomerResponseDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerUpdateDTO);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<CustomerResponseDTO>> GetActiveCustomersAsync();
        Task<IEnumerable<CustomerResponseDTO>> GetCustomersByTotalSpentAsync(decimal minAmount);
        Task<CustomerResponseDTO> UpdateCustomerStatusAsync(int customerId);
    }
}
