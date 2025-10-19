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

        /// <summary>
        /// Lấy tất cả nhật ký ấp theo Id lô trứng.
        /// </summary>
        [HttpGet("EggBatch/{eggBatchId}")]
        public async Task<IActionResult> GetAllByEggBatchId(int eggBatchId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _recordService.GetAllByEggBatchIdAsync(eggBatchId, pageIndex, pageSize);

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
                return Error($"Lỗi khi lấy danh sách nhật ký ấp: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin nhật ký ấp theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var record = await _recordService.GetByIdAsync(id);
                if (record == null)
                    return GetError("Không tìm thấy bản ghi nhật ký ấp.");

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
                return Error("Đã xảy ra lỗi khi lấy thông tin nhật ký ấp.");
            }
        }

        /// <summary>
        /// Tạo mới bản ghi nhật ký ấp.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IncubationDailyRecordRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _recordService.CreateAsync(dto);

                return GetSuccess(new { message = "Tạo bản ghi nhật ký ấp thành công.", data = created });
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
                return Error($"Không thể tạo bản ghi mới: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật bản ghi nhật ký ấp.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] IncubationDailyRecordRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _recordService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy bản ghi để cập nhật.");

                return GetSuccess("Cập nhật bản ghi nhật ký ấp thành công.");
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
                return Error($"Lỗi khi cập nhật bản ghi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa bản ghi nhật ký ấp.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _recordService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy bản ghi cần xóa.");

                return GetSuccess("Xóa bản ghi nhật ký ấp thành công.");
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
                return Error("Đã xảy ra lỗi khi xóa bản ghi.");
            }
        }
    }
}
