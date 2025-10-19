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

        /// <summary>
        /// Lấy danh sách tất cả các ghi nhận phân loại (có phân trang).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _recordService.GetAllAsync(pageIndex, pageSize);

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
                return Error($"Lỗi khi lấy danh sách ghi nhận phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy chi tiết một ghi nhận phân loại theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var record = await _recordService.GetByIdAsync(id);
                if (record == null)
                    return GetError("Không tìm thấy ghi nhận phân loại.");

                return GetSuccess(record);
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
                return Error("Đã xảy ra lỗi khi lấy ghi nhận phân loại.");
            }
        }

        /// <summary>
        /// Tạo mới một ghi nhận phân loại (tăng cấp giai đoạn phân loại).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassificationRecordRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _recordService.CreateAsync(dto);

                return GetSuccess(new
                {
                    message = "Tạo ghi nhận phân loại thành công.",
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
                return Error($"Không thể tạo ghi nhận phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin ghi nhận phân loại.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassificationRecordUpdateRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _recordService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy ghi nhận để cập nhật.");

                return GetSuccess("Cập nhật ghi nhận phân loại thành công.");
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
                return Error($"Lỗi khi cập nhật ghi nhận phân loại: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa ghi nhận phân loại theo ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _recordService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy ghi nhận cần xóa.");

                return GetSuccess("Xóa ghi nhận phân loại thành công.");
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
                return Error($"Đã xảy ra lỗi khi xóa ghi nhận phân loại: {ex.Message}");
            }
        }
    }
}
