using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WaterAlertDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WaterAlertController : BaseAPIController
    {
        private readonly IWaterAlertService _waterAlertService;

        public WaterAlertController(IWaterAlertService waterAlertService)
        {
            _waterAlertService = waterAlertService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(
         [FromQuery] WaterAlertFilterRequestDTO? filter,
         [FromQuery] int pageIndex = 1,
         [FromQuery] int pageSize = 10)
        {
            var data = await _waterAlertService.GetAllWaterAlertAsync(filter, pageIndex, pageSize);
            return GetPagedSuccess(data); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var alert = await _waterAlertService.GetByIdAsync(id);
            if (alert == null)
                return GetError("Không tìm thấy cảnh báo nước.");

            return GetSuccess(alert);
        }
     /*   [HttpPost]
        public async Task<IActionResult> Create([FromBody] WaterAlertRequestDTO dto)
        {
            try
            {
                int userId = UserId; // lấy từ BaseAPIController
                var createdAlert = await _waterAlertService.CreateAsync(userId, dto);
                return Success(createdAlert, "Tạo cảnh báo nước thành công.");
            }
            catch (Exception ex)
            {
                return GetError(ex.Message);
            }
        }
*/


            [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id)
        {
            int userId = UserId;
            var success = await _waterAlertService.ResolveAsync(id, userId);
            if (!success)
                return GetError("Không tìm thấy hoặc không thể xử lý cảnh báo.");

            return Success(success, "Đã xử lý cảnh báo nước thành công.");
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _waterAlertService.DeleteAsync(id);
            if (!success)
                return GetError("Không tìm thấy cảnh báo để xóa.");

            return Success(success, "Xóa cảnh báo nước thành công.");
        }
    }
}
