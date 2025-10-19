using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FryFishController : BaseAPIController
    {
        private readonly IFryFishService _fryFishService;

        public FryFishController(IFryFishService fryFishService)
        {
            _fryFishService = fryFishService;
        }

        /// <summary>
        /// Lấy danh sách tất cả cá bột (có phân trang).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _fryFishService.GetAllAsync(pageIndex, pageSize);

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
                return Error($"Lỗi khi lấy danh sách cá bột: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin cá bột theo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var fryFish = await _fryFishService.GetByIdAsync(id);
                if (fryFish == null)
                    return GetError("Không tìm thấy thông tin cá bột.");

                return GetSuccess(fryFish);
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
                return Error("Đã xảy ra lỗi khi lấy thông tin cá bột.");
            }
        }

        /// <summary>
        /// Tạo mới lô cá bột.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FryFishRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _fryFishService.CreateAsync(dto);

                return GetSuccess(new
                {
                    message = "Tạo lô cá bột thành công.",
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
                return Error($"Không thể tạo lô cá bột: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin lô cá bột.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FryFishUpdateRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _fryFishService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy lô cá bột để cập nhật.");

                return GetSuccess("Cập nhật lô cá bột thành công.");
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
                return Error($"Lỗi khi cập nhật cá bột: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa lô cá bột theo ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _fryFishService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy lô cá bột cần xóa.");

                return GetSuccess("Xóa lô cá bột thành công.");
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
                return Error($"Đã xảy ra lỗi khi xóa cá bột: {ex.Message}");
            }
        }
    }
}
