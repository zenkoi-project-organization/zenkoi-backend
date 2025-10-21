using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BreedingProcessController : BaseAPIController
    {
        private readonly IBreedingProcessService _service;

        public BreedingProcessController(IBreedingProcessService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBreedingProcesses(
            [FromQuery] BreedingProcessFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1, 
            [FromQuery] int pageSize = 10)
        {
            var data = await _service.GetAllBreedingProcess(filter ?? new BreedingProcessFilterRequestDTO(), pageIndex, pageSize);
            return GetPagedSuccess(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBreedingById(int id)
        {
            var breeding = await _service.GetBreedingById(id);
            if (breeding == null)
                return GetError("Không tìm thấy quy trình sinh sản.");

            return GetSuccess(breeding);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBreeding([FromBody] BreedingProcessRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var created = await _service.AddBreeding(dto);
            return SaveSuccess(created, "Tạo quy trình sinh sản thành công.");
        }

        [HttpGet("offspring")]
        public async Task<IActionResult> GetOffspringInbreeding([FromQuery] int maleId, [FromQuery] int femaleId)
        {

            var result = await _service.GetOffspringInbreedingAsync(maleId, femaleId);
            return GetSuccess(new
            {
                MaleId = maleId,
                FemaleId = femaleId,
                InbreedingCoefficient = result
            });
        }

        [HttpGet("individual/{koiId:int}")]
        public async Task<IActionResult> GetIndividualInbreeding(int koiId)
        {
            var result = await _service.GetIndividualInbreedingAsync(koiId);
            return GetSuccess(new
            {
                KoiId = koiId,
                InbreedingCoefficient = result
            });
        }
    }
}
