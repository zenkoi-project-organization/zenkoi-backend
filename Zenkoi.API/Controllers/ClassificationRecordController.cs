using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationRecordController : BaseAPIController
    {
        private readonly IClassificationRecordService _recordService;

        public ClassificationRecordController(IClassificationRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _recordService.GetAllAsync(pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
         
            var record = await _recordService.GetByIdAsync(id);
            if (record == null)
                return GetError("Không tìm thấy ghi nhận phân loại.");

            return GetSuccess(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassificationRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại thành công.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassificationRecordUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _recordService.UpdateAsync(id, dto);

            return Success(updated, "Cập nhật ghi nhận phân loại thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _recordService.DeleteAsync(id);

            return Success(deleted, "Xóa ghi nhận phân loại thành công.");
        }
    }
}
