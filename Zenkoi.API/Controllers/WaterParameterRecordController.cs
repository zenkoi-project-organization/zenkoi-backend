using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterParameterRecordController : BaseAPIController
    {
        private readonly IWaterParameterRecordService _service;

        public WaterParameterRecordController(IWaterParameterRecordService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] WaterParameterRecordFilterDTO? filter,
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
                return GetError("Không tìm thấy bản ghi thông số nước.");
            return GetSuccess(data);
        }

        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WaterParameterRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var created = await _service.CreateAsync(UserId, dto);
                return SaveSuccess(created, "Ghi nhận thông số nước thành công.");
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WaterParameterRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated == null)
                    return GetError("Không tìm thấy bản ghi để cập nhật.");
                return Success(updated, "Cập nhật thông số thành công.");
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy bản ghi để xóa.");
            return Success(true, "Xóa bản ghi thành công.");
        }
    }
}