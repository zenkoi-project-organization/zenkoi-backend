using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<IActionResult> GetAllAreas(
            [FromQuery] AreaFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _areaService.GetAllAsync(filter ?? new AreaFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

      
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var data = await _areaService.GetByIdAsync(id);
            return GetSuccess(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();
            
            var created = await _areaService.CreateAsync(dto);
             return SaveSuccess(created); ;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _areaService.UpdateAsync(id, dto);
             return GetSuccess(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var deleted = await _areaService.DeleteAsync(id);
            return Success(deleted, "Xóa khu vực thành công.");
        }
    }
}
