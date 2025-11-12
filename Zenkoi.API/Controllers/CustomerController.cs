using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.CustomerDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Queries;

namespace Zenkoi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : BaseAPIController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
     

        /// <summary>
        /// Get customer by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Customer details</returns>
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

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public async Task<ActionResult<ResponseApiDTO>> GetAllCustomers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetAllCustomersAsync(pageIndex, pageSize);
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

        /// <summary>
        /// Update customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="customerUpdateDTO">Updated customer data</param>
        /// <returns>Updated customer</returns>
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

        /// <summary>
        /// Delete customer (soft delete)
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success status</returns>
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

        /// <summary>
        /// Get active customers only
        /// </summary>
        /// <returns>List of active customers</returns>
        [HttpGet("active")]
        public async Task<ActionResult<ResponseApiDTO>> GetActiveCustomers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetActiveCustomersAsync(pageIndex, pageSize);
                var response = new PagingDTO<CustomerResponseDTO>(result);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Active customers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving active customers"
                });
            }
        }

        /// <summary>
        /// Get customers by minimum total spent amount
        /// </summary>
        /// <param name="minAmount">Minimum total spent amount</param>
        /// <returns>List of customers</returns>
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

        /// <summary>
        /// Update customer statistics (total orders and total spent)
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Updated customer</returns>
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
