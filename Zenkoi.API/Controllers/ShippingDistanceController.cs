using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ShippingDistanceDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingDistanceController : BaseAPIController
    {
        private readonly IShippingDistanceService _shippingDistanceService;

        public ShippingDistanceController(IShippingDistanceService shippingDistanceService)
        {
            _shippingDistanceService = shippingDistanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _shippingDistanceService.GetAllAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách khoảng cách vận chuyển: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _shippingDistanceService.GetByIdAsync(id);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin khoảng cách vận chuyển: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShippingDistanceRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingDistanceService.CreateAsync(dto);
                return SaveSuccess(result, "Tạo khoảng cách vận chuyển thành công");
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi tạo khoảng cách vận chuyển: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingDistanceRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _shippingDistanceService.UpdateAsync(id, dto);
                return SaveSuccess(result, "Cập nhật khoảng cách vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi cập nhật khoảng cách vận chuyển: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _shippingDistanceService.DeleteAsync(id);
                return SaveSuccess(result, "Xóa khoảng cách vận chuyển thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi xóa khoảng cách vận chuyển: {ex.Message}");
            }
        }
    }
}
