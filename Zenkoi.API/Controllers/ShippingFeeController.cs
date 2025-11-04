using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<ShippingFeeBreakdownDTO>> CalculateShippingFee([FromBody] CalculateShippingFeeRequestDTO request)
        {
            try
            {
                var result = await _shippingFeeCalculationService.CalculateShippingFeeAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An error occurred while calculating shipping fee: {ex.Message}" });
            }
        }
    }
}
