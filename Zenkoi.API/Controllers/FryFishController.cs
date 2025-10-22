using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FryFishController : BaseAPIController
    {
        private readonly IFryFishService _fryFishService;

        public FryFishController(IFryFishService fryFishService)
        {
            _fryFishService = fryFishService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] FryFishFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _fryFishService.GetAllAsync(filter ?? new FryFishFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var fryFish = await _fryFishService.GetByIdAsync(id);
            if (fryFish == null)
                return GetError("Không tìm thấy thông tin cá bột.");

            return GetSuccess(fryFish);
        }
        [HttpGet("by-breeding/{breedId}")]
        public async Task<IActionResult> GetByBreedId(int breedId)
        {

            var result = await _fryFishService.GetEggBatchByBreedId(breedId);
            if (result == null)
                return GetError("Không tìm thấy lô cá bột.");

            return GetSuccess(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FryFishRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _fryFishService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo lô cá bột thành công.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FryFishUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _fryFishService.UpdateAsync(id, dto);
          
            return Success(updated, "Cập nhật lô cá bột thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _fryFishService.DeleteAsync(id);
       
            return Success(deleted, "Xóa lô cá bột thành công.");
        }
    }
}
