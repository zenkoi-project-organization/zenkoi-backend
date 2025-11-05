using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingFeeController : BaseAPIController
    {
        private readonly IShippingFeeCalculationService _shippingFeeCalculationService;

        public ShippingFeeController(IShippingFeeCalculationService shippingFeeCalculationService)
        {
            _shippingFeeCalculationService = shippingFeeCalculationService;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<ResponseApiDTO>> CalculateShippingFee([FromBody] CalculateShippingFeeRequestDTO request)
        {
            try
            {
                var result = await _shippingFeeCalculationService.CalculateShippingFeeAsync(request);
                return Ok(new ResponseApiDTO
                {
                    IsSuccess = true,
                    Message = "Shipping fee calculated successfully",
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApiDTO
                {
                    IsSuccess = false,
                    Message = $"An error occurred while calculating shipping fee: {ex.Message}"
                });
            }
        }
    }
}
