using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
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
        public async Task<IActionResult> GetAllPonds(
            [FromQuery] PondFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _pondService.GetAllPondsAsync(filter ?? new PondFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondById(int id)
        {
       
            var data = await _pondService.GetByIdAsync(id);
            if (data == null)
                return GetError("Không tìm thấy ao.");

            return GetSuccess(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePond([FromBody] PondRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();
            int userId = UserId; 

            var created = await _pondService.CreateAsync(userId,dto);
            return SaveSuccess(created, "Tạo ao thành công.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePond(int id, [FromBody] PondUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();
            int userId = UserId;
            var updated = await _pondService.UpdateAsync(id, UserId,dto);
            if (updated == null)
                return GetError("Không tìm thấy ao để cập nhật.");

            return Success(updated, "Cập nhật ao thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePond(int id)
        {
            var deleted = await _pondService.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy ao để xóa.");

            return Success(deleted, "Xóa ao thành công.");
        }
        [HttpGet("{pondId}/koifish")]
        public async Task<IActionResult> GetKoiFishByPond(int pondId)
        {
            var koiList = await _pondService.GetAllKoiFishInPond(pondId);
            if (koiList == null || !koiList.Any())
                return GetError("Không tìm thấy cá trong hồ này.");

            return GetSuccess(koiList);
        }
    }
}
