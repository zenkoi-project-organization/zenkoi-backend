using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PondController : BaseAPIController
    {
        private readonly IPondService _pondService;

        public PondController(IPondService pondService)
        {
            _pondService = pondService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPonds()
        {
            try
            {
                var data = await _pondService.GetAllAsync();
                if (data == null || !data.Any())
                    return GetError("Không tìm thấy ao nào.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy danh sách ao.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondById(int id)
        {
            try
            {
              
                var data = await _pondService.GetByIdAsync(id);
                if (data == null)
                    return GetError("Không tìm thấy ao.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy thông tin ao.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePond([FromBody] PondRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.PondName))
                    return GetError("Tên ao không được để trống.");

         
                var created = await _pondService.CreateAsync(dto);
                
               
                if (created == null)
                    return GetError("Không thể tạo ao mới.");

                return GetSuccess(new { message = "Tạo ao thành công.", data = created });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePond(int id, [FromBody] PondRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _pondService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy ao cần cập nhật.");

                return GetSuccess("Cập nhật ao thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình cập nhật ao.");
            }
        }

    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePond(int id)
        {
            try
            {
                var success = await _pondService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy ao cần xóa.");

                return GetSuccess("Xóa ao thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xóa ao.");
            }
        }
    }
}
