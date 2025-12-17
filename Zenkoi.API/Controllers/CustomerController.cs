using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Queries;

namespace Zenkoi.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : BaseAPIController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
     

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ResponseApiDTO>> GetCustomerByUserId(int userId)
        {
            try
            {
                var result = await _customerService.GetCustomerByUserIdAsync(userId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer retrieved successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving customer"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseApiDTO>> GetAllCustomers(
            [FromQuery] CustomerFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetAllCustomersAsync(filter ?? new CustomerFilterRequestDTO(), pageIndex, pageSize);
                var response = new PagingDTO<CustomerResponseDTO>(result);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = $"An error occurred while retrieving customers: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseApiDTO>> UpdateCustomer(int id, [FromBody] CustomerUpdateDTO customerUpdateDTO)
        {
            try
            {
                var result = await _customerService.UpdateCustomerAsync(id, customerUpdateDTO);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer updated successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating customer"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseApiDTO>> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (result)
                {
                    return Ok(new ResponseApiDTO
                    {
                        IsSuccess = true,
                        Message = "Customer deleted successfully",
                        Result = result
                    });
                }
                else
                {
                    return NotFound(new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Customer not found"
                    });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting customer"
                });
            }
        }
          
        [HttpGet("by-total-spent/{minAmount}")]
        public async Task<ActionResult<ResponseApiDTO>> GetCustomersByTotalSpent(decimal minAmount, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetCustomersByTotalSpentAsync(minAmount, pageIndex, pageSize);
                var response = new PagingDTO<CustomerResponseDTO>(result);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving customers"
                });
            }
        }

        [HttpPut("{customerId}/update-stats")]
        public async Task<ActionResult<ResponseApiDTO>> UpdateCustomerStatus(int customerId)
        {
            try
            {
                var result = await _customerService.UpdateCustomerStatusAsync(customerId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer statistics updated successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while updating customer statistics"
                });
            }
        }
    }
}
