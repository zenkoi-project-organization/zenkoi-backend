using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDTO> CreateCustomerAsync(CustomerRequestDTO customerRequestDTO);
        Task<CustomerResponseDTO> GetCustomerByIdAsync(int id);
        Task<CustomerResponseDTO> GetCustomerByUserIdAsync(int userId);
        Task<IEnumerable<CustomerResponseDTO>> GetAllCustomersAsync(QueryOptions<Customer>? queryOptions = null);
        Task<CustomerResponseDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerUpdateDTO);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<CustomerResponseDTO>> GetActiveCustomersAsync();
        Task<IEnumerable<CustomerResponseDTO>> GetCustomersByTotalSpentAsync(decimal minAmount);
        Task<CustomerResponseDTO> UpdateCustomerStatsAsync(int customerId);
    }
}
