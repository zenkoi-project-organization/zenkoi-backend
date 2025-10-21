using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarietyController : BaseAPIController
    {
        private readonly IVarietyService _varietyService;

        public VarietyController(IVarietyService varietyService)
        {
            _varietyService = varietyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVarieties([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _varietyService.GetAllVarietiesAsync(pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVarietyById(int id)
        {
            var data = await _varietyService.GetByIdAsync(id);
            if (data == null)
                return NotFound(new { message = "Không tìm thấy giống cá." });

            return GetSuccess(data);
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateVariety([FromBody] VarietyRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _varietyService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo giống cá thành công.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVariety(int id, [FromBody] VarietyRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _varietyService.UpdateAsync(id, dto);
            
            return Success(updated, "cập nhật giống cá thành công");
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariety(int id)
        {
            var deleted = await _varietyService.DeleteAsync(id);
     
            return Success(deleted, "Xóa giống cá thành công.");
        }
    }
}
