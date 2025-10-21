using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FrySurvivalRecordController : BaseAPIController
    {
        private readonly IFrySurvivalRecordService _frySurvivalService;

        public FrySurvivalRecordController(IFrySurvivalRecordService frySurvivalService)
        {
            _frySurvivalService = frySurvivalService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] FrySurvivalRecordFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _frySurvivalService.GetAllVarietiesAsync(filter ?? new FrySurvivalRecordFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
         

            var record = await _frySurvivalService.GetByIdAsync(id);
            if (record == null)
                return GetError("Không tìm thấy bản ghi nhận.");

            return GetSuccess(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FrySurvivalRecordRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _frySurvivalService.CreateAsync(dto);
            return SaveSuccess(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FrySurvivalRecordUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _frySurvivalService.UpdateAsync(id, dto);
           
            return Success(updated, "Cập nhật ghi nhận tỷ lệ sống thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _frySurvivalService.DeleteAsync(id);
        
            return Success(deleted, "Xóa ghi nhận tỷ lệ sống thành công.");
        }
    }
}
