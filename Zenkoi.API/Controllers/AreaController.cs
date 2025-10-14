using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : BaseAPIController
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAreas()
        {
            try
            {
                var data = await _areaService.GetAllAsync();
                if (data == null || !data.Any())
                    return GetError("Không tìm thấy khu vực nào.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình lấy dữ liệu khu vực.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            try
            {
                if (id <= 0)
                    return GetError("Id phải là số nguyên dương.");

                var data = await _areaService.GetByIdAsync(id);
                if (data == null)
                    return GetError("Không tìm thấy khu vực.");

                return GetSuccess(data);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaRequestDTO dto)
        {
            try
            {
                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                if (string.IsNullOrWhiteSpace(dto.AreaName))
                    return GetError("Tên khu vực không được để trống.");

                var created = await _areaService.CreateAsync(dto);
                if (created == null)
                    return GetError("Không thể tạo khu vực mới.");

                return GetSuccess(new { message = "Tạo khu vực thành công.", data = created });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình tạo khu vực.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaRequestDTO dto)
        {
            try
            {
                if (id < 0)
                    return GetError("Id phải là số nguyên dương.");

                if (dto == null)
                    return GetError("Dữ liệu không hợp lệ.");

                var success = await _areaService.UpdateAsync(id, dto);
                if (!success)
                    return GetError("Không tìm thấy khu vực cần cập nhật.");

                return GetSuccess("Cập nhật khu vực thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình cập nhật khu vực.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            try
            {
                if (id <= 0)
                    return GetError("Id phải là số nguyên dương.");

                var success = await _areaService.DeleteAsync(id);
                if (!success)
                    return GetError("Không tìm thấy khu vực cần xóa.");

                return GetSuccess("Xóa khu vực thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error("Đã xảy ra lỗi trong quá trình xóa khu vực.");
            }
        }
    }
}
