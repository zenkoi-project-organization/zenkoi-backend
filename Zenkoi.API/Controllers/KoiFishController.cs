using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
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
        public async Task<IActionResult> GetAllKoiFish(
            [FromQuery] KoiFishFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _koiFishService.GetAllKoiFishAsync(filter ?? new KoiFishFilterRequestDTO(), pageIndex, pageSize);

            

            return GetPagedSuccess(result);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetKoiFishById(int id)
        {
            var koi = await _koiFishService.GetByIdAsync(id);
            if (koi == null)
                return GetNotFound($"Không tìm thấy cá koi với ID {id}.");

            return GetSuccess(koi);
        }

      
        [HttpPost]
        public async Task<IActionResult> CreateKoiFish([FromBody] KoiFishRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _koiFishService.CreateAsync(dto);
            if (created == null)
                return SaveError();

            return SaveSuccess(created);
        }

      
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateKoiFish(int id, [FromBody] KoiFishRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _koiFishService.UpdateAsync(id, dto);
            if (!updated)
                return GetNotFound($"Không tìm thấy cá koi cần cập nhật (ID = {id}).");

            return SaveSuccess($"Cập nhật cá koi thành công (ID = {id}).");
        }

       
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteKoiFish(int id)
        {
            var deleted = await _koiFishService.DeleteAsync(id);
            if (!deleted)
                return GetNotFound($"Không tìm thấy cá koi cần xóa (ID = {id}).");

            return SaveSuccess($"Đã xóa cá koi thành công (ID = {id}).");
        }
        [HttpGet("family/{id}")]
        public async Task<IActionResult> GetFamilyTree(int id)
        {
            var result = await _koiFishService.GetFamilyTreeAsync(id);
            return GetSuccess(result);
        }
    }
}
