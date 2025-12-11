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
        
            int? userId = null;
            try
            {
                userId = UserId;
            }
            catch
            {
               
            }

            var result = await _koiFishService.GetAllKoiFishAsync(
                filter ?? new KoiFishFilterRequestDTO(),
                pageIndex,
                pageSize,
                userId);

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
        [HttpGet("scan-rfid/{RFID}")]
        public async Task<IActionResult> GetKoiFishByRFID( string RFID)
        {
            var koi = await _koiFishService.ScanRFID(RFID);
            if (koi == null)
                return GetNotFound($"Không tìm thấy cá koi với RFID {RFID}.");

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
        public async Task<IActionResult> UpdateKoiFish(int id, [FromBody] KoiFishUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _koiFishService.UpdateAsync(id, dto);
            if (!updated)
                return GetNotFound($"Không tìm thấy cá koi cần cập nhật (ID = {id}).");

            return SaveSuccess($"Cập nhật cá koi thành công (ID = {id}).");
        }
        [HttpPut("koi-spawn/{id:int}")]
        public async Task<IActionResult> UpdateFishSpawn (int id)
        {

            var updated = await _koiFishService.UpdateKoiSpawning(id);
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

        [HttpPut("{id:int}/transfer/{pondId:int}")]
        public async Task<IActionResult> TransferKoiFish(int id, int pondId)
        {
            var result = await _koiFishService.TransferFish(id, pondId);
            if (!result)
                return SaveError();

            return SaveSuccess($"Chuyển cá koi (ID = {id}) sang hồ (ID = {pondId}) thành công.");
        }
        [HttpGet("{id:int}/breeding-history")]
        public async Task<IActionResult> GetBreedingHistory(int id)
        {
            var result = await _koiFishService.GetKoiBreedingHistory(id);
            return GetSuccess(result);
        }
    }
}
