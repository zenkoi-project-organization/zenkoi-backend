using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiFishController : BaseAPIController
    {
        private readonly IKoiFishService _koiFishService;

        public KoiFishController(IKoiFishService koiFishService)
        {
            _koiFishService = koiFishService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllKoiFish()
        {
            try
            {
                var data = await _koiFishService.GetAllAsync();
                if (data == null || !data.Any())
                    return GetError("Không tìm thấy cá koi nào.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy danh sách cá koi.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetKoiFishById(int id)
        {
            try
            {
                var data = await _koiFishService.GetByIdAsync(id);
                if (data == null)
                    return GetError("Không tìm thấy cá koi với ID này.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy thông tin cá koi.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateKoiFish([FromBody] KoiFishRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var created = await _koiFishService.CreateAsync(dto);
                if (created == null)
                    return GetError("Không thể tạo cá koi mới.");

                return GetSuccess(new { message = "Tạo cá koi thành công.", data = created });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKoiFish(int id, [FromBody] KoiFishRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _koiFishService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy cá koi cần cập nhật.");

                return GetSuccess("Cập nhật cá koi thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình cập nhật cá koi.");
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKoiFish(int id)
        {
            try
            {
                var success = await _koiFishService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy cá koi cần xóa.");

                return GetSuccess("Xóa cá koi thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xóa cá koi.");
            }
        }
    }
}
