using AutoMapper;
using Microsoft.Extensions.Options;
using Zenkoi.BLL.DTOs.CustomerAddressDTOs;
using Zenkoi.BLL.Helpers.Config;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<CustomerAddress> _customerAddressRepo;
        private readonly IRepoBase<Customer> _customerRepo;
        private readonly IMapService _mapService;
        private readonly FarmLocationConfiguration _farmLocation;

        public CustomerAddressService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMapService mapService,
            IOptions<FarmLocationConfiguration> farmLocation
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapService = mapService;
            _customerAddressRepo = _unitOfWork.GetRepo<CustomerAddress>();
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _farmLocation = farmLocation.Value;
        }

        public async Task<CustomerAddressResponseDTO> CreateCustomerAddressAsync(CustomerAddressRequestDTO requestDTO, int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            var customerAddress = _mapper.Map<CustomerAddress>(requestDTO);
            customerAddress.CustomerId = customerId; 
            customerAddress.CreatedAt = DateTime.UtcNow;
            customerAddress.IsActive = true;

            if (requestDTO.IsDefault)
            {
                await UnsetAllDefaultAddressesAsync(customerId);
            }
         
            if (requestDTO.Latitude.HasValue && requestDTO.Longitude.HasValue)
            {
                try
                {
                    var distanceKm = await _mapService.CalculateDistanceAsync(
                        _farmLocation.Latitude,
                        _farmLocation.Longitude,
                        requestDTO.Latitude.Value,
                        requestDTO.Longitude.Value
                    );

                    customerAddress.DistanceFromFarmKm = distanceKm;
                    customerAddress.DistanceCalculatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {            
                    customerAddress.DistanceFromFarmKm = null;
                    customerAddress.DistanceCalculatedAt = null;
                }
            }

            await _customerAddressRepo.CreateAsync(customerAddress);
            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerAddressByIdAsync(customerAddress.Id);
        }

        public async Task<CustomerAddressResponseDTO> GetCustomerAddressByIdAsync(int id)
        {
            var address = await _customerAddressRepo.GetSingleAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.Id == id)
                .WithInclude(a => a.Customer)
                .WithInclude(a => a.Customer.ApplicationUser)
                .Build());

            if (address == null)
            {
                throw new ArgumentException("Customer address not found");
            }

            var responseDTO = _mapper.Map<CustomerAddressResponseDTO>(address);
            responseDTO.CustomerName = address.Customer?.ApplicationUser?.FullName ?? string.Empty;

            return responseDTO;
        }

        public async Task<IEnumerable<CustomerAddressResponseDTO>> GetAllCustomerAddressesAsync()
        {
            var addresses = await _customerAddressRepo.GetAllAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.IsActive == true) 
                .WithInclude(a => a.Customer)
                .WithInclude(a => a.Customer.ApplicationUser)
                .WithOrderBy(a => a.OrderByDescending(x => x.CreatedAt))
                .Build());

            var responseDTOs = _mapper.Map<IEnumerable<CustomerAddressResponseDTO>>(addresses);

            foreach (var dto in responseDTOs)
            {
                var address = addresses.FirstOrDefault(a => a.Id == dto.Id);
                if (address != null)
                {
                    dto.CustomerName = address.Customer?.ApplicationUser?.FullName ?? string.Empty;
                }
            }

            return responseDTOs;
        }

        public async Task<IEnumerable<CustomerAddressResponseDTO>> GetAddressesByCustomerIdAsync(int customerId)
        {
            var addresses = await _customerAddressRepo.GetAllAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.CustomerId == customerId && a.IsActive == true)
                .WithInclude(a => a.Customer)
                .WithInclude(a => a.Customer.ApplicationUser)
                .WithOrderBy(a => a.OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.CreatedAt))
                .Build());

            var responseDTOs = _mapper.Map<IEnumerable<CustomerAddressResponseDTO>>(addresses);

            foreach (var dto in responseDTOs)
            {
                var address = addresses.FirstOrDefault(a => a.Id == dto.Id);
                if (address != null)
                {
                    dto.CustomerName = address.Customer?.ApplicationUser?.FullName ?? string.Empty;
                }
            }

            return responseDTOs;
        }

        public async Task<CustomerAddressResponseDTO> GetDefaultAddressByCustomerIdAsync(int customerId)
        {
            var address = await _customerAddressRepo.GetSingleAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.CustomerId == customerId && a.IsDefault == true && a.IsActive == true)
                .WithInclude(a => a.Customer)
                .WithInclude(a => a.Customer.ApplicationUser)
                .Build());

            if (address == null)
            {
                throw new ArgumentException("No default address found for this customer");
            }

            var responseDTO = _mapper.Map<CustomerAddressResponseDTO>(address);
            responseDTO.CustomerName = address.Customer?.ApplicationUser?.FullName ?? string.Empty;

            return responseDTO;
        }

        public async Task<CustomerAddressResponseDTO> UpdateCustomerAddressAsync(int id, CustomerAddressUpdateDTO updateDTO)
        {
            var address = await _customerAddressRepo.GetByIdAsync(id);
            if (address == null)
            {
                throw new ArgumentException("Customer address not found");
            }

            if (updateDTO.FullAddress != null)
                address.FullAddress = updateDTO.FullAddress;

            if (updateDTO.City != null)
                address.City = updateDTO.City;

            if (updateDTO.Ward != null)
                address.Ward = updateDTO.Ward;

            if (updateDTO.StreetAddress != null)
                address.StreetAddress = updateDTO.StreetAddress;

            if (updateDTO.Latitude.HasValue)
                address.Latitude = updateDTO.Latitude;

            if (updateDTO.Longitude.HasValue)
                address.Longitude = updateDTO.Longitude;

            if (updateDTO.RecipientPhone != null)
                address.RecipientPhone = updateDTO.RecipientPhone;

            if (updateDTO.IsDefault.HasValue && updateDTO.IsDefault.Value)
            {
                await UnsetAllDefaultAddressesAsync(address.CustomerId);
                address.IsDefault = true;
            }
            else if (updateDTO.IsDefault.HasValue)
            {
                address.IsDefault = updateDTO.IsDefault.Value;
            }

            if (updateDTO.IsActive.HasValue)
                address.IsActive = updateDTO.IsActive.Value;

            address.UpdatedAt = DateTime.UtcNow;
         
            if ((updateDTO.Latitude.HasValue || updateDTO.Longitude.HasValue) &&
                address.Latitude.HasValue && address.Longitude.HasValue)
            {
                try
                {
                    var distanceKm = await _mapService.CalculateDistanceAsync(
                        _farmLocation.Latitude,
                        _farmLocation.Longitude,
                        address.Latitude.Value,
                        address.Longitude.Value
                    );

                    address.DistanceFromFarmKm = distanceKm;
                    address.DistanceCalculatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {              
                    address.DistanceFromFarmKm = null;
                    address.DistanceCalculatedAt = null;
                }
            }

            await _customerAddressRepo.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerAddressByIdAsync(id);
        }

        public async Task<bool> DeleteCustomerAddressAsync(int id)
        {
            var address = await _customerAddressRepo.GetByIdAsync(id);
            if (address == null)
            {
                return false;
            }

            address.IsActive = false;
            address.UpdatedAt = DateTime.UtcNow;

            await _customerAddressRepo.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<CustomerAddressResponseDTO> SetDefaultAddressAsync(int customerId, int addressId)
        {
            var address = await _customerAddressRepo.GetSingleAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.Id == addressId && a.CustomerId == customerId)
                .Build());

            if (address == null)
            {
                throw new ArgumentException("Address not found or does not belong to this customer");
            }

            if (!address.IsActive)
            {
                throw new ArgumentException("Cannot set inactive address as default");
            }

            await UnsetAllDefaultAddressesAsync(customerId);

            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            await _customerAddressRepo.UpdateAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerAddressByIdAsync(addressId);
        }

        public async Task<CustomerAddressResponseDTO> CalculateDistanceAsync(int addressId)
        {
            var address = await _customerAddressRepo.GetByIdAsync(addressId);
            if (address == null)
            {
                throw new ArgumentException("Customer address not found");
            }

            if (!address.Latitude.HasValue || !address.Longitude.HasValue)
            {
                throw new ArgumentException("Address does not have coordinates set");
            }

            try
            {
                var distanceKm = await _mapService.CalculateDistanceAsync(
                      _farmLocation.Latitude,
                      _farmLocation.Longitude,
                      address.Latitude.Value,
                      address.Longitude.Value
                  );


                address.DistanceFromFarmKm = distanceKm;
                address.DistanceCalculatedAt = DateTime.UtcNow;
                address.UpdatedAt = DateTime.UtcNow;

                await _customerAddressRepo.UpdateAsync(address);
                await _unitOfWork.SaveChangesAsync();

                return await GetCustomerAddressByIdAsync(addressId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to calculate distance using Map API: {ex.Message}", ex);
            }
        }

        private async Task UnsetAllDefaultAddressesAsync(int customerId)
        {
            var defaultAddresses = await _customerAddressRepo.GetAllAsync(new QueryBuilder<CustomerAddress>()
                .WithPredicate(a => a.CustomerId == customerId && a.IsDefault == true)
                .Build());

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
                addr.UpdatedAt = DateTime.UtcNow;
                await _customerAddressRepo.UpdateAsync(addr);
            }
        }
    }
}
