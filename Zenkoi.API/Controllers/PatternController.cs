using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.PatternDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternController : BaseAPIController
    {
        private readonly IPatternService _patternService;

        public PatternController(IPatternService patternService)
        {
            _patternService = patternService;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _patternService.GetAllAsync(pageIndex, pageSize);
            return GetPagedSuccess(data);
        }


      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pattern = await _patternService.GetByIdAsync(id);
            if (pattern == null)
                return GetError("Không tìm thấy pattern.");

            return GetSuccess(pattern);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PatternRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _patternService.CreateAsync(dto);
            return SaveSuccess(created);
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PatternRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _patternService.UpdateAsync(id, dto);
            if (!updated)
                return GetError("Không tìm thấy pattern để cập nhật.");

            return Success(updated, "Cập nhật pattern thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _patternService.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy pattern để xóa.");

            return Success(deleted, "Xóa pattern thành công.");
        }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignPattern([FromBody] AssignPatternDTO dto)
        {
            var ok = await _patternService.AssignPatternToVarietyAsync(dto.VarietyId, dto.PatternId);
            return ok ? Success(ok, "Gán pattern cho variety thành công.")
                      : GetError("Không thể gán pattern.");
        }

        // 2. Gỡ pattern khỏi variety
        [HttpDelete("remove/{varietyId}/{patternId}")]
        public async Task<IActionResult> RemovePattern(int varietyId, int patternId)
        {
            var ok = await _patternService.RemovePatternFromVarietyAsync(varietyId, patternId);
            return ok ? Success(ok, "Xóa pattern khỏi variety thành công.")
                      : GetError("Không tìm thấy liên kết để xóa.");
        }

        // 3. Lấy danh sách pattern theo variety
        [HttpGet("by-variety/{varietyId}")]
        public async Task<IActionResult> GetPatternsByVariety(int varietyId)
        {
            var list = await _patternService.GetPatternsByVarietyAsync(varietyId);
            return GetPagedSuccess(list);
        }

        // 4. Lấy danh sách variety theo pattern
        [HttpGet("{patternId}/varieties")]
        public async Task<IActionResult> GetVarietiesByPattern(int patternId)
        {
            var list = await _patternService.GetVarietiesByPatternAsync(patternId);
            return GetPagedSuccess(list);
        }
    }
}
    
