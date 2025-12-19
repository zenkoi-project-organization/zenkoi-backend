using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
//using Newtonsoft.Json;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs;
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
        private readonly IKoiFishService _fishService;
        public BreedingProcessController(IBreedingProcessService service, IBreedingAdvisorService advisorService, IKoiFishService fishService)
        {
            _service = service;
            _advisorService = advisorService;
            _fishService =  fishService;
        }
        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpPut("spawned/{id:int}")]
        public async Task<IActionResult> UpdateSpawned(int id)
        {
            var breeding = await _service.UpdateSpawnedStatus(id);
            return Success(breeding,"cập nhật thành công");
        }
        [Authorize(Roles = "Manager")]
        [HttpPut("cancel/{id:int}")]
        public async Task<IActionResult> UpdateCancel(int id,[FromBody] string note)
        {
            var breeding = await _service.CancelBreeding(id,note);
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
        [Authorize(Roles = "Manager")]
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

        [HttpGet("{id}/koi-fishes")]
        public async Task<IActionResult> GetKoiFishesByBreeding(int id)
        {
            var koiList = await _service.GetAllKoiFishByBreedingProcessAsync(id);
            return GetSuccess(koiList);
        }
        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpPost("recommend")]
        public async Task<IActionResult> Recommend([FromBody] BreedingRequestInputDTO input)
        {
            var allParents = await _service.GetParentsWithPerformanceAsync(
                input.MinHatchRate,
                input.MinSurvivalRate,
                input.TargetVariety
            );

            Console.WriteLine("===== ALL PARENTS =====");
            Console.WriteLine(JsonSerializer.Serialize(
                allParents,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            ));
            Console.WriteLine("===== END ALL PARENTS =====");

            if (allParents == null || !allParents.Any())
            {
                return GetSuccess(new AIBreedingResponseDTO
                {
                    RecommendedPairs = new List<BreedingPairResult>()
                });
            }

            var request = new BreedingRequestDTO
            {
                TargetVariety = input.TargetVariety,
                Priority = input.Priority,
                MinHatchRate = input.MinHatchRate,
                MinSurvivalRate = input.MinSurvivalRate,
                IsMutation = input.IsMutation,
                PotentialParents = allParents
            };

            var result = await _advisorService.RecommendPairsAsync(request);

            return GetSuccess(result);
        }
        [Authorize(Roles = "Manager,FarmStaff")]
        [HttpPost("analyze-pair")]
        public async Task<IActionResult> AnalyzePair([FromBody] AIPairAnalysisSimpleRequestDTO req)
        {
         
            
            var male = await _fishService.GetAnalysisAsync(req.MaleId);
            var female = await _fishService.GetAnalysisAsync(req.FemaleId);
         
            if (male == null || female == null)
                return Error("Không tìm thấy dữ liệu cá tương ứng.");

            if (male.Gender?.ToLower() != "male" || female.Gender?.ToLower() != "female")
                return Error("Dữ liệu không hợp lệ: cần 1 cá đực (Male) và 1 cá cái (Female).");

            var fullRequest = new AIPairAnalysisRequestDTO
            {
                Male = male,
                Female = female
            };
           var result = await _advisorService.AnalyzePairAsync(fullRequest);
            return GetSuccess(result);
        }
    }
    }

