using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrySurvivalRecordController : BaseAPIController
    {
        private readonly IFrySurvivalRecordService _frySurvivalService;

        public FrySurvivalRecordController(IFrySurvivalRecordService frySurvivalService)
        {
            _frySurvivalService = frySurvivalService;
        }

        /// <summary>
        /// Lấy danh sách ghi nhận tỷ lệ sống của cá bột (có phân trang).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _frySurvivalService.GetAllVarietiesAsync(pageIndex, pageSize);

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
                return Error($"Lỗi khi lấy danh sách ghi nhận tỷ lệ sống: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy chi tiết một ghi nhận tỷ lệ sống.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var record = await _frySurvivalService.GetByIdAsync(id);
                if (record == null)
                    return GetError("Không tìm thấy bản ghi nhận.");

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
                return Error("Đã xảy ra lỗi khi lấy bản ghi nhận.");
            }
        }

        /// <summary>
        /// Tạo mới ghi nhận tỷ lệ sống cho bầy cá.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FrySurvivalRecordRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _frySurvivalService.CreateAsync(dto);

                return GetSuccess(new
                {
                    message = "Tạo ghi nhận tỷ lệ sống thành công.",
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
                return Error($"Không thể tạo ghi nhận mới: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin ghi nhận tỷ lệ sống.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FrySurvivalRecordUpdateRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _frySurvivalService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy bản ghi nhận để cập nhật.");

                return GetSuccess("Cập nhật ghi nhận tỷ lệ sống thành công.");
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
                return Error($"Lỗi khi cập nhật ghi nhận: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa bản ghi nhận tỷ lệ sống.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _frySurvivalService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy bản ghi nhận cần xóa.");

                return GetSuccess("Xóa ghi nhận tỷ lệ sống thành công.");
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
                return Error($"Đã xảy ra lỗi khi xóa ghi nhận: {ex.Message}");
            }
        }
    }
}
