using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.AreaDTOs;
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

        /// <summary>
        /// Lấy danh sách tất cả khu vực
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAreas()
        {
            var data = await _areaService.GetAllAsync();
            return GetSuccess(data);
        }

        /// <summary>
        /// Lấy thông tin khu vực theo Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            var data = await _areaService.GetByIdAsync(id);
            return GetSuccess(data);
        }

        /// <summary>
        /// Tạo khu vực mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] AreaRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();
            
            var created = await _areaService.CreateAsync(dto);
             return GetSuccess(created); ;
        }

        /// <summary>
        /// Cập nhật khu vực
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, [FromBody] AreaRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _areaService.UpdateAsync(id, dto);
             return GetSuccess(updated);
        }

        /// <summary>
        /// Xóa khu vực
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var deleted = await _areaService.DeleteAsync(id);
            return Success(deleted, "Xóa khu vực thành công.");
        }
    }
}
