using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.CustomerAddressDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerAddressController : BaseAPIController
    {
        private readonly ICustomerAddressService _customerAddressService;

        public CustomerAddressController(ICustomerAddressService customerAddressService)
        {
            _customerAddressService = customerAddressService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseApiDTO>> CreateCustomerAddress([FromBody] CustomerAddressRequestDTO requestDTO)
        {
            try
            {
                var result = await _customerAddressService.CreateCustomerAddressAsync(requestDTO, UserId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer address created successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseApiDTO
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
                    Message = "An error occurred while creating customer address"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseApiDTO>> GetCustomerAddressById(int id)
        {
            try
            {
                var result = await _customerAddressService.GetCustomerAddressByIdAsync(id);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer address retrieved successfully",
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
                    Message = "An error occurred while retrieving customer address"
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ResponseApiDTO>> GetAllCustomerAddresses()
        {
            try
            {
                var result = await _customerAddressService.GetAllCustomerAddressesAsync();
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer addresses retrieved successfully",
                    Result = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving customer addresses"
                });
            }
        }

        [HttpGet("customer/me")]
        public async Task<ActionResult<ResponseApiDTO>> GetAddressesByCustomerId()
        {
            try
            {
                var result = await _customerAddressService.GetAddressesByCustomerIdAsync(UserId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer addresses retrieved successfully",
                    Result = result
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while retrieving customer addresses"
                });
            }
        }

        [HttpGet("customer/me/default")]
        public async Task<ActionResult<ResponseApiDTO>> GetDefaultAddressByCustomerId()
        {
            try
            {
                var result = await _customerAddressService.GetDefaultAddressByCustomerIdAsync(UserId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Default customer address retrieved successfully",
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
                    Message = "An error occurred while retrieving default customer address"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseApiDTO>> UpdateCustomerAddress(int id, [FromBody] CustomerAddressUpdateDTO updateDTO)
        {
            try
            {
                var result = await _customerAddressService.UpdateCustomerAddressAsync(id, updateDTO);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Customer address updated successfully",
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
                    Message = "An error occurred while updating customer address"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseApiDTO>> DeleteCustomerAddress(int id)
        {
            try
            {
                var result = await _customerAddressService.DeleteCustomerAddressAsync(id);
                if (result)
                {
                    return Ok(new ResponseApiDTO
                    {
                        IsSuccess = true,
                        Message = "Customer address deleted successfully",
                        Result = result
                    });
                }
                else
                {
                    return NotFound(new ResponseApiDTO
                    {
                        IsSuccess = false,
                        Message = "Customer address not found"
                    });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting customer address"
                });
            }
        }

        [HttpPut("customer/me/set-default/{addressId}")]
        public async Task<ActionResult<ResponseApiDTO>> SetDefaultAddress(int addressId)
        {
            try
            {
                var result = await _customerAddressService.SetDefaultAddressAsync(UserId, addressId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Default address set successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseApiDTO
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
                    Message = "An error occurred while setting default address"
                });
            }
        }

        [HttpPost("{addressId}/calculate-distance")]
        public async Task<ActionResult<ResponseApiDTO>> CalculateDistance(int addressId)
        {
            try
            {
                var result = await _customerAddressService.CalculateDistanceAsync(addressId);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Distance calculated successfully",
                    Result = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseApiDTO
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
                    Message = "An error occurred while calculating distance"
                });
            }
        }
    }
}
