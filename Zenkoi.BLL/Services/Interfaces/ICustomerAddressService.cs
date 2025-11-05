using Zenkoi.BLL.DTOs.CustomerAddressDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<CustomerAddressResponseDTO> CreateCustomerAddressAsync(CustomerAddressRequestDTO requestDTO);
        Task<CustomerAddressResponseDTO> GetCustomerAddressByIdAsync(int id);
        Task<IEnumerable<CustomerAddressResponseDTO>> GetAllCustomerAddressesAsync();
        Task<IEnumerable<CustomerAddressResponseDTO>> GetAddressesByCustomerIdAsync(int customerId);
        Task<IEnumerable<CustomerAddressResponseDTO>> GetActiveAddressesByCustomerIdAsync(int customerId);
        Task<CustomerAddressResponseDTO> GetDefaultAddressByCustomerIdAsync(int customerId);
        Task<CustomerAddressResponseDTO> UpdateCustomerAddressAsync(int id, CustomerAddressUpdateDTO updateDTO);
        Task<bool> DeleteCustomerAddressAsync(int id);
        Task<CustomerAddressResponseDTO> SetDefaultAddressAsync(int customerId, int addressId);
        Task<CustomerAddressResponseDTO> CalculateDistanceAsync(int addressId);
    }
}
