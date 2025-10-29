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

        [HttpGet("egg-batch/{eggBatchId}")]
        public async Task<IActionResult> GetAllByEggBatchId(
            int eggBatchId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
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

        [HttpPost("v2")]
        public async Task<IActionResult> CreateV2([FromBody] IncubationDailyRecordRequestV2DTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV2Async(dto);
            return SaveSuccess(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IncubationDailyRecordUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _recordService.UpdateAsync(id, dto);
            return Success(updated, "Cập nhật bản ghi nhật ký ấp thành công.");
        }

        [HttpPut("v2/{id}")]
        public async Task<IActionResult> UpdateV2(int id, [FromBody] IncubationDailyRecordUpdateV2RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _recordService.UpdateV2Async(id, dto);
            return Success(updated, "Cập nhật bản ghi nhật ký ấp (V2) thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _recordService.DeleteAsync(id);
            return Success(deleted, "Xóa bản ghi nhật ký ấp thành công.");
        }

        [HttpGet("egg-batch/{eggBatchId}/summary")]
        public async Task<IActionResult> GetSummaryByEggBatchId(int eggBatchId)
        {
            var summary = await _recordService.GetSummaryByEggBatchIdAsync(eggBatchId);
            return GetSuccess(summary);
        }
    }
}
