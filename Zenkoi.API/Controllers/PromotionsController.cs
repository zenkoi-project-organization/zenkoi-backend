using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.PromotionDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PromotionsController : BaseAPIController
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] PromotionFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _promotionService.GetAllAsync(filter ?? new PromotionFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _promotionService.GetByIdAsync(id);
            if (data == null)
                return GetError("Không tìm thấy khuyến mãi.");

            return GetSuccess(data);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([FromBody] PromotionRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var created = await _promotionService.CreateAsync(dto);
                return SaveSuccess(created, "Tạo khuyến mãi thành công.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var updated = await _promotionService.UpdateAsync(id, dto);
                if (!updated)
                    return GetError("Không tìm thấy khuyến mãi để cập nhật.");

                return Success(updated, "Cập nhật khuyến mãi thành công.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _promotionService.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy khuyến mãi để xóa.");

            return Success(deleted, "Xóa khuyến mãi thành công.");
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var promotion = await _promotionService.GetCurrentActivePromotionAsync();
            return GetSuccess(promotion);
        }

        [HttpPatch("{id:int}/toggle-active")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ToggleIsActive(int id)
        {
            try
            {
                var result = await _promotionService.ToggleIsActiveAsync(id);
                return SaveSuccess(result, "Thay đổi trạng thái khuyến mãi thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
        }
    }
}
