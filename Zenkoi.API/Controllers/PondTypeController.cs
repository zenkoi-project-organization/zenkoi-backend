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
        public async Task<IActionResult> GetAllPondTypes([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var data = await _pondTypeService.GetAllAsync(pageIndex, pageSize);
            return GetPagedSuccess(data);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPondTypeById(int id)
        {
         
            var data = await _pondTypeService.GetByIdAsync(id);
            if (data == null)
                return GetError("Không tìm thấy loại ao.");

            return GetSuccess(data);
        }


        [HttpPost]
        public async Task<IActionResult> CreatePondType([FromBody] PondTypeRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _pondTypeService.CreateAsync(dto);
            return SaveSuccess(created);
        }

   
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePondType(int id, [FromBody] PondTypeRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _pondTypeService.UpdateAsync(id, dto);
            if (updated == null)
                return GetError("Không tìm thấy loại ao để cập nhật.");

            return Success(updated, "Cập nhật loại ao thành công.");
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePondType(int id)
        {
            var deleted = await _pondTypeService.DeleteAsync(id);
            if (!deleted)
                return GetError("Không tìm thấy loại ao để xóa.");

            return Success(deleted, "Xóa loại ao thành công.");
        }
    }
}
