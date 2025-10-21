using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncubationDailyRecordController : BaseAPIController
    {
        private readonly IIncubationDailyRecordService _recordService;

        public IncubationDailyRecordController(IIncubationDailyRecordService recordService)
        {
            _recordService = recordService;
        }


        [HttpGet("EggBatch/{eggBatchId}")]
        public async Task<IActionResult> GetAllByEggBatchId(int eggBatchId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _recordService.GetAllByEggBatchIdAsync(eggBatchId, pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
       
            var record = await _recordService.GetByIdAsync(id);
            if (record == null)
                return GetError("Không tìm thấy bản ghi nhật ký ấp.");

            return GetSuccess(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IncubationDailyRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateAsync(dto);
            return SaveSuccess(created);
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IncubationDailyRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _recordService.UpdateAsync(id, dto);
            if (updated == null)
                return GetError("Không tìm thấy bản ghi để cập nhật.");

            return Success(updated, "Cập nhật bản ghi nhật ký ấp thành công.");
        }

     
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _recordService.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy bản ghi để xóa.");

            return Success(deleted, "Xóa bản ghi nhật ký ấp thành công.");
        }
    }
}
