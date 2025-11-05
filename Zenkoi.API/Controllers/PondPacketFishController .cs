using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondPacketFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Paging;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PondPacketFishController : BaseAPIController
    {
        private readonly IPondPacketFishService _pondPacketFishService;

        public PondPacketFishController(IPondPacketFishService pondPacketFishService)
        {
            _pondPacketFishService = pondPacketFishService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPondPacketFish([FromQuery] PondPacketFishFilterRequestDTO? filter, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _pondPacketFishService.GetAllPondPacketFishAsync(filter ?? new PondPacketFishFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondPacketFishById(int id)
        {
            var data = await _pondPacketFishService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Không tìm thấy lô cá." });

            return GetSuccess(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePondPacketFish([FromBody] PondPacketFishRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _pondPacketFishService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo lô cá thành công.");
        }

        [HttpPut("{id}/transfer")]
        public async Task<IActionResult> TransferPondPacketFish(int id, [FromBody] PondPacketFishUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var transferred = await _pondPacketFishService.TranferPacket(id, dto);
            return Success(transferred, "Chuyển lô cá sang hồ mới thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePondPacketFish(int id)
        {
            var deleted = await _pondPacketFishService.DeleteAsync(id);
            return Success(deleted, "Xóa lô cá thành công.");
        }
    }
}
