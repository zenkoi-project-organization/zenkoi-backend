using AutoMapper;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
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

        public async Task<PaginatedList<CustomerResponseDTO>> GetAllCustomersAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _customerRepo.Get(new QueryBuilder<Customer>()
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(3))
                .WithOrderBy(c => c.OrderByDescending(x => x.CreatedAt))
                .WithTracking(false)
                .Build());

            var pagedCustomers = await PaginatedList<Customer>.CreateAsync(query, pageIndex, pageSize);
            var result = _mapper.Map<List<CustomerResponseDTO>>(pagedCustomers);
            return new PaginatedList<CustomerResponseDTO>(result, pagedCustomers.TotalItems, pageIndex, pageSize);
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

        public async Task<PaginatedList<CustomerResponseDTO>> GetActiveCustomersAsync(int pageIndex = 1, int pageSize = 10)
        {
            var query = _customerRepo.Get(new QueryBuilder<Customer>()
                .WithPredicate(c => c.IsActive == true)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(3))
                .WithOrderBy(c => c.OrderByDescending(x => x.CreatedAt))
                .WithTracking(false)
                .Build());

            var pagedCustomers = await PaginatedList<Customer>.CreateAsync(query, pageIndex, pageSize);
            var result = _mapper.Map<List<CustomerResponseDTO>>(pagedCustomers);
            return new PaginatedList<CustomerResponseDTO>(result, pagedCustomers.TotalItems, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CustomerResponseDTO>> GetCustomersByTotalSpentAsync(decimal minAmount, int pageIndex = 1, int pageSize = 10)
        {
            var query = _customerRepo.Get(new QueryBuilder<Customer>()
                .WithPredicate(c => c.TotalSpent >= minAmount && c.IsActive == true)
                .WithInclude(c => c.ApplicationUser)
                .WithInclude(c => c.Orders.Take(3))
                .WithOrderBy(c => c.OrderByDescending(x => x.TotalSpent))
                .WithTracking(false)
                .Build());

            var pagedCustomers = await PaginatedList<Customer>.CreateAsync(query, pageIndex, pageSize);
            var result = _mapper.Map<List<CustomerResponseDTO>>(pagedCustomers);
            return new PaginatedList<CustomerResponseDTO>(result, pagedCustomers.TotalItems, pageIndex, pageSize);
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
