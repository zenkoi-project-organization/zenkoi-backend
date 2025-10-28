using AutoMapper;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Customer> _customerRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;
        private readonly IRepoBase<Order> _orderRepo;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            _orderRepo = _unitOfWork.GetRepo<Order>();
        }

        public async Task CreateCustomerProfileAsync(int userId)
        {
          
            var existingCustomer = await _customerRepo.GetByIdAsync(userId);
            if (existingCustomer != null)
                return; 
      
            var customer = new Customer
            {
                Id = userId,
                IsActive = true,
                TotalOrders = 0,
                TotalSpent = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _customerRepo.CreateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CustomerResponseDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                .WithPredicate(c => c.Id == id)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(5))
                .Build());

            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<CustomerResponseDTO> GetCustomerByUserIdAsync(int userId)
        {
            var customer = await _customerRepo.GetSingleAsync(new QueryBuilder<Customer>()
                .WithPredicate(c => c.Id == userId)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(5))
                .Build());

            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetAllCustomersAsync(QueryOptions<Customer>? queryOptions = null)
        {
            if (queryOptions == null)
            {
                var customers = await _customerRepo.GetAllAsync(new QueryBuilder<Customer>()
                    .WithInclude(c => c.ApplicationUser)
                    .WithInclude(c => c.Orders.Take(3))
                    .WithOrderBy(c => c.OrderByDescending(x => x.CreatedAt))
                    .Build());

                return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
            }

            var customersWithCustomOptions = await _customerRepo.GetAllAsync(queryOptions);
            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customersWithCustomOptions);
        }

        public async Task<CustomerResponseDTO> UpdateCustomerAsync(int id, CustomerUpdateDTO customerUpdateDTO)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            _mapper.Map(customerUpdateDTO, customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepo.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerByIdAsync(id);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepo.GetByIdAsync(id);
            if (customer == null)
            {
                return false;
            }         
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepo.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetActiveCustomersAsync()
        {
            var customers = await _customerRepo.GetAllAsync(new QueryBuilder<Customer>()
                .WithPredicate(c => c.IsActive == true)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(3))
                .WithOrderBy(c => c.OrderByDescending(x => x.CreatedAt))
                .Build());

            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        }

        public async Task<IEnumerable<CustomerResponseDTO>> GetCustomersByTotalSpentAsync(decimal minAmount)
        {
            var customers = await _customerRepo.GetAllAsync(new QueryBuilder<Customer>()
                .WithPredicate(c => c.TotalSpent >= minAmount && c.IsActive == true)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(3))
                .WithOrderBy(c => c.OrderByDescending(x => x.TotalSpent))
                .Build());

            return _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);
        }

        public async Task<CustomerResponseDTO> UpdateCustomerStatusAsync(int customerId)
        {
            var customer = await _customerRepo.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new ArgumentException("Customer not found");
            }

            var orders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CustomerId == customerId)
                .Build());

            customer.TotalOrders = orders.Count();
            customer.TotalSpent = orders.Sum(o => o.TotalAmount);
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepo.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return await GetCustomerByIdAsync(customerId);
        }
    }
}
