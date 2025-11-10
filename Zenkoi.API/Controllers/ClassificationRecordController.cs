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

        /// <summary>
        /// 🔹 Lấy danh sách tất cả ghi nhận phân loại (có phân trang + filter)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ClassificationRecordFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var data = await _recordService.GetAllAsync(filter ?? new ClassificationRecordFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        /// <summary>
        /// 🔹 Lấy chi tiết ghi nhận phân loại theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var record = await _recordService.GetByIdAsync(id);
            if (record == null)
                return GetError("Không tìm thấy ghi nhận phân loại.");

            return GetSuccess(record);
        }

        /// <summary>
        /// 🔹 Phân loại lần 1 (tạo ghi nhận mới)
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] ClassificationRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 1 thành công.");
        }

        /// <summary>
        /// 🔹 Phân loại lần 2 (có tính mutation type & mutation rate)
        /// </summary>
        [HttpPost("create-v1")]
        public async Task<IActionResult> CreateV1([FromBody] ClassificationRecordV1RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV1Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 2 thành công (v1).");
        }

        /// <summary>
        /// 🔹 Phân loại lần 3 (lọc High quality fish)
        /// </summary>
        [HttpPost("create-v2")]
        public async Task<IActionResult> CreateV2([FromBody] ClassificationRecordV2RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV2Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần 3 thành công (v2).");
        }

        /// <summary>
        /// 🔹 Phân loại lần 4 (lọc Show fish)
        /// </summary>
        [HttpPost("create-v3")]
        public async Task<IActionResult> CreateV3([FromBody] ClassificationRecordV3RequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _recordService.CreateV3Async(dto);
            return SaveSuccess(created, "Tạo ghi nhận phân loại lần cuối (v3) thành công.");
        }

        /// <summary>
        /// 🔹 Cập nhật ghi nhận phân loại (chỉ cho bản ghi mới nhất)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassificationRecordUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _recordService.UpdateAsync(id, dto);
            return Success(updated, "Cập nhật ghi nhận phân loại thành công.");
        }

        /// <summary>
        /// 🔹 Xóa ghi nhận phân loại
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _recordService.DeleteAsync(id);
            return Success(deleted, "Xóa ghi nhận phân loại thành công.");
        }

        /// <summary>
        /// 🔹 Lấy tổng kết phân loại (summary theo bầy)
        /// </summary>
        [HttpGet("summary/{classificationStageId}")]
        public async Task<IActionResult> GetSummary(int classificationStageId)
        {
            var summary = await _recordService.GetSummaryAsync(classificationStageId);
            return Success(summary, "Lấy tổng kết phân loại thành công.");
        }
    }
}
