using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateCustomerProfileAsync(int userId);
        Task<CustomerResponseDTO> GetCustomerByUserIdAsync(int userId);
        Task<PaginatedList<CustomerResponseDTO>> GetAllCustomersAsync(CustomerFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<CustomerResponseDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerUpdateDTO);
        Task<bool> DeleteCustomerAsync(int id);
        //Task<PaginatedList<CustomerResponseDTO>> GetActiveCustomersAsync(int pageIndex = 1, int pageSize = 10);
        Task<PaginatedList<CustomerResponseDTO>> GetCustomersByTotalSpentAsync(decimal minAmount, int pageIndex = 1, int pageSize = 10);
        Task<CustomerResponseDTO> UpdateCustomerStatusAsync(int customerId);
    }
}
