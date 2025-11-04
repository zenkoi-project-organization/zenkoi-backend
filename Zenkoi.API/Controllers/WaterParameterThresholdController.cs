using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterParameterThresholdController : BaseAPIController
    {
        private readonly IWaterParameterThresholdService _service;

        public WaterParameterThresholdController(IWaterParameterThresholdService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] WaterParameterThresholdFilterDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllAsync(filter, pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
                return GetError("Không tìm thấy ngưỡng thông số.");
            return GetSuccess(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WaterParameterThresholdRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var created = await _service.CreateAsync(dto);
                return SaveSuccess(created, "Tạo ngưỡng thành công.");
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WaterParameterThresholdRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                    return GetError("Không tìm thấy ngưỡng để cập nhật.");
                return Success(updated, "Cập nhật ngưỡng thành công.");
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy ngưỡng để xóa.");
            return Success(true, "Xóa ngưỡng thành công.");
        }
    }
}