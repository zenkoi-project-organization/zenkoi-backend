using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
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
        public async Task<IActionResult> GetAll(
            [FromQuery] ClassificationRecordFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _recordService.GetAllAsync(filter ?? new ClassificationRecordFilterRequestDTO(), pageIndex, pageSize);
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateClassificationRecord([FromBody] ClassificationRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 1 thành công.");
        }

        [HttpPost("create-v1")]
        public async Task<IActionResult> CreateV1([FromBody] ClassificationRecordV1RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV1Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 2 thành công (v1).");
        }

        [HttpPost("create-v2")]
        public async Task<IActionResult> CreateClassificationRecordV2([FromBody] ClassificationRecordV2RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV2Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 3 thành công (v2).");
        }

  
        [HttpPost("create-v3")]
        public async Task<IActionResult> CreateClassificationRecordV3([FromBody] ClassificationRecordV3RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV3Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần cuối (v3) thành công.");
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

     
        [HttpGet("summary/{classificationStageId}")]
        public async Task<IActionResult> GetSummary(int classificationStageId)
        {
            var summary = await _recordService.GetSummaryAsync(classificationStageId);
            return Success(summary, "Lấy tổng kết phân loại thành công.");
        }
    }
}
