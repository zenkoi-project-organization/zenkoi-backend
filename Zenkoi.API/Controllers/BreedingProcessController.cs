using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BreedingProcessController : BaseAPIController
    {
        private readonly IBreedingProcessService _service;
        private readonly IBreedingAdvisorService _advisorService;
        public BreedingProcessController(IBreedingProcessService service, IBreedingAdvisorService advisorService)
        {
            _service = service;
            _advisorService = advisorService;
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpawnedById(int id)
        {
            var breeding = await _service.UpdateStatus(id);
            return Success(breeding,"cập nhật thành công");
        }
        [HttpGet("{koiFishId}/breeding-parent-history")]
        public async Task<IActionResult> GetKoiFishBreedingStats(int koiFishId)
        {
            var stats = await _service.GetKoiFishParentStatsAsync(koiFishId);
            return GetSuccess(stats);
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
        [HttpGet("detail/{id:int}")]
        public async Task<IActionResult> GetBreedingDetailById(int id)
        {
            var breeding = await _service.GetDetailBreedingById(id);
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

        [HttpPost("recommend")]
        public async Task<IActionResult> Recommend([FromBody] BreedingRequestInputDTO input)
        {
            var allParents = await _service.GetParentsWithPerformanceAsync();

            Console.WriteLine("check cá :", JsonConvert.SerializeObject(allParents, Formatting.Indented));

            var filteredParents = allParents
                .Where(p => string.Equals(p.Variety, input.TargetVariety, StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            Console.WriteLine( "check cá :",JsonConvert.SerializeObject(filteredParents, Formatting.Indented));
            var request = new BreedingRequestDTO
            {
                TargetVariety = input.TargetVariety,
                Priority = input.Priority,
                DesiredPattern = input.DesiredPattern,
                DesiredBodyShape = input.DesiredBodyShape,
                MinHatchRate = input.MinHatchRate,
                MinSurvivalRate = input.MinSurvivalRate,
                MinHighQualifiedRate = input.MinHighQualifiedRate,
                PotentialParents = filteredParents
            };

            Console.WriteLine(JsonConvert.SerializeObject(request, Formatting.Indented));


            var result = await _advisorService.RecommendPairsAsync(request);
            return GetSuccess(result);
        }
    }
}
