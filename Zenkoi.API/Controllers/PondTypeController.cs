using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PondTypeController : BaseAPIController
    {
        private readonly IPondTypeService _pondTypeService;

        public PondTypeController(IPondTypeService pondTypeService)
        {
            _pondTypeService = pondTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPondTypes()
        {
            try
            {
                var data = await _pondTypeService.GetAllAsync();
                if (data == null || !data.Any())
                    return GetError("Không tìm thấy loại ao nào.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy dữ liệu loại ao.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondTypeById(int id)
        {
            try
            {
                if (id < 0)
                    return GetError("Id phải là số nguyên dương.");

                var data = await _pondTypeService.GetByIdAsync(id);
                if (data == null)
                    return GetError("Không tìm thấy loại ao.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy thông tin loại ao.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePondType([FromBody] PondTypeRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.TypeName))
                    return GetError("Tên loại ao không được để trống.");

                var created = await _pondTypeService.CreateAsync(dto);
                if (created == null)
                    return GetError("Không thể tạo loại ao mới.");

                return GetSuccess(new { message = "Tạo loại ao thành công.", data = created });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình tạo loại ao.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePondType(int id, [FromBody] PondTypeRequestDTO dto)
        {
            try
            {
                if (id <= 0)
                    return GetError("Id phải là số nguyên dương.");

                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _pondTypeService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy loại ao cần cập nhật.");

                return GetSuccess("Cập nhật loại ao thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình cập nhật loại ao.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePondType(int id)
        {
            try
            {
                if (id <= 0)
                    return GetError("Id phải là số nguyên dương.");

                var success = await _pondTypeService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy loại ao cần xóa.");

                return GetSuccess("Xóa loại ao thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xóa loại ao.");
            }
        }
    }
}
