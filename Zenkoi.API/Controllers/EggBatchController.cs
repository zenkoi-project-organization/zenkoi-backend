using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EggBatchController : BaseAPIController
    {
        private readonly IEggBatchService _eggBatchService;

        public EggBatchController(IEggBatchService eggBatchService)
        {
            _eggBatchService = eggBatchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _eggBatchService.GetAllEggBatchAsync(pageIndex, pageSize);
            return GetPagedSuccess(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var result = await _eggBatchService.GetByIdAsync(id);
            if (result == null)
                return GetError("Không tìm thấy lô trứng.");

            return GetSuccess(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EggBatchRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _eggBatchService.CreateAsync(dto);
            return SaveSuccess(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EggBatchUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _eggBatchService.UpdateAsync(id, dto);
      
            return Success(updated, "Cập nhật lô trứng thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _eggBatchService.DeleteAsync(id);
         
            return Success(deleted, "Xóa lô trứng thành công.");
        }
    }
}
