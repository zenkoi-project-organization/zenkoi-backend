using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ShippingBoxDTOs;
using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingBoxController : BaseAPIController
    {
        private readonly IShippingBoxService _shippingBoxService;
        private readonly IShippingCalculatorService _shippingCalculatorService;

        public ShippingBoxController(
            IShippingBoxService shippingBoxService,
            IShippingCalculatorService shippingCalculatorService)
        {
            _shippingBoxService = shippingBoxService;
            _shippingCalculatorService = shippingCalculatorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _shippingBoxService.GetAllAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _shippingBoxService.GetByIdAsync(id);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShippingBoxRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingBoxService.CreateAsync(dto);
                return SaveSuccess(result, "Tạo hộp vận chuyển thành công");
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingBoxRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingBoxService.UpdateAsync(id, dto);
                return SaveSuccess(result, "Cập nhật hộp vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _shippingBoxService.DeleteAsync(id);
                return SaveSuccess(result, "Xóa hộp vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa hộp vận chuyển: {ex.Message}");
            }
        }

        [HttpPost("rules")]
        public async Task<IActionResult> AddRule([FromBody] ShippingBoxRuleRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingBoxService.AddRuleAsync(dto);
                return SaveSuccess(result, "Thêm quy tắc vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi thêm quy tắc vận chuyển: {ex.Message}");
            }
        }

        [HttpPut("rules/{ruleId:int}")]
        public async Task<IActionResult> UpdateRule(int ruleId, [FromBody] ShippingBoxRuleUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingBoxService.UpdateRuleAsync(ruleId, dto);
                return SaveSuccess(result, "Cập nhật quy tắc vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật quy tắc vận chuyển: {ex.Message}");
            }
        }

        [HttpDelete("rules/{ruleId:int}")]
        public async Task<IActionResult> DeleteRule(int ruleId)
        {
            try
            {
                var result = await _shippingBoxService.DeleteRuleAsync(ruleId);
                return SaveSuccess(result, "Xóa quy tắc vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa quy tắc vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("rules/{ruleId:int}")]
        public async Task<IActionResult> GetRuleById(int ruleId)
        {
            try
            {
                var result = await _shippingBoxService.GetRuleByIdAsync(ruleId);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin quy tắc vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("{boxId:int}/rules")]
        public async Task<IActionResult> GetRulesByBoxId(int boxId)
        {
            try
            {
                var result = await _shippingBoxService.GetRulesByBoxIdAsync(boxId);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách quy tắc vận chuyển: {ex.Message}");
            }
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
    }
}
