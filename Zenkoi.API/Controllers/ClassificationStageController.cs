using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationStageController : BaseAPIController
    {
        private readonly IClassificationStageService _classificationService;

        public ClassificationStageController(IClassificationStageService classificationService)
        {
            _classificationService = classificationService;
        }

        /// <summary>
        /// Lấy danh sách các giai đoạn phân loại cá (có phân trang).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _classificationService.GetAllAsync(pageIndex, pageSize);

                var response = new
                {
                    result.PageIndex,
                    result.TotalPages,
                    result.TotalItems,
                    result.HasNextPage,
                    result.HasPreviousPage,
                    Data = result
                };

                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi khi lấy danh sách giai đoạn phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin giai đoạn phân loại theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var classification = await _classificationService.GetByIdAsync(id);
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(classification, Newtonsoft.Json.Formatting.Indented));

                return GetSuccess(classification);
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi khi lấy thông tin giai đoạn phân loại.");
            }
        }

        /// <summary>
        /// Tạo mới giai đoạn phân loại cá.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassificationStageCreateRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _classificationService.CreateAsync(dto);

                return GetSuccess(new
                {
                    message = "Tạo giai đoạn phân loại thành công.",
                    data = created
                });
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Không thể tạo giai đoạn phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin giai đoạn phân loại.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassificationStageUpdateRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _classificationService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy giai đoạn phân loại để cập nhật.");

                return GetSuccess("Cập nhật giai đoạn phân loại thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi khi cập nhật giai đoạn phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa giai đoạn phân loại cá theo ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _classificationService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy giai đoạn phân loại cần xóa.");

                return GetSuccess("Xóa giai đoạn phân loại thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Đã xảy ra lỗi khi xóa giai đoạn phân loại: {ex.Message}");
            }
        }
    }
}
