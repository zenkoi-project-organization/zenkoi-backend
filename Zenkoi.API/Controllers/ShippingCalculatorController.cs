using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingCalculatorController : BaseAPIController
    {
        private readonly IShippingCalculatorService _shippingCalculatorService;

        public ShippingCalculatorController(IShippingCalculatorService shippingCalculatorService)
        {
            _shippingCalculatorService = shippingCalculatorService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateShipping([FromBody] ShippingCalculationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingCalculatorService.CalculateShipping(request);
                return GetSuccess(result);
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tính toán chi phí vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("boxes")]
        public async Task<IActionResult> GetAvailableBoxes()
        {
            try
            {
                var result = await _shippingCalculatorService.GetAvailableBoxes();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("boxes/{id:int}")]
        public async Task<IActionResult> GetBoxById(int id)
        {
            try
            {
                var result = await _shippingCalculatorService.GetBoxById(id);
                if (result == null)
                    return GetNotFound("Không tìm thấy hộp vận chuyển");

                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin hộp vận chuyển: {ex.Message}");
            }
        }
    }
}
