using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassificationStageController : BaseAPIController
    {
        private readonly IClassificationStageService _classificationService;

        public ClassificationStageController(IClassificationStageService classificationService)
        {
            _classificationService = classificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ClassificationStageFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _classificationService.GetAllAsync(filter ?? new ClassificationStageFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var classification = await _classificationService.GetByIdAsync(id);
            
            return GetSuccess(classification);
        }

        [HttpGet("by-breeding/{breedId}")]
        public async Task<IActionResult> GetByBreedId(int breedId)
        {

            var result = await _classificationService.GetEggBatchByBreedId(breedId);
            if (result == null)
                return GetError("Không tìm thấy lô cá tuyển chọn.");

            return GetSuccess(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassificationStageCreateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _classificationService.CreateAsync(dto);
            return SaveSuccess(created, "Tạo giai đoạn phân loại thành công.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassificationStageUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var updated = await _classificationService.UpdateAsync(id, dto);


            return Success(updated, "Cập nhật giai đoạn phân loại thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _classificationService.DeleteAsync(id);


            return Success(deleted, "Xóa giai đoạn phân loại thành công.");
        }
    }
}
