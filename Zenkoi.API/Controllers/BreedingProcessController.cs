using MailKit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;
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
        [HttpPut("spawned/{id:int}")]
        public async Task<IActionResult> UpdateSpawnedById(int id)
        {
            var breeding = await _service.UpdateStatus(id);
            return Success(breeding,"cập nhật thành công");
        }
        [HttpPut("cancel/{id:int}")]
        public async Task<IActionResult> UpdateCancelById(int id)
        {
            var breeding = await _service.CancelBreeding(id);
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

        [HttpGet("{id}/koi-fishes")]
        public async Task<IActionResult> GetKoiFishesByBreeding(int id)
        {
            var koiList = await _service.GetAllKoiFishByBreedingProcessAsync(id);
            return GetSuccess(koiList);
        }

        [HttpPost("recommend")]
        public async Task<IActionResult> Recommend([FromBody] BreedingRequestInputDTO input)
        {
            var allParents = await _service.GetParentsWithPerformanceAsync(input.TargetVariety);

            var request = new BreedingRequestDTO
            {
                TargetVariety = input.TargetVariety,
                Priority = input.Priority,
                MinHatchRate = input.MinHatchRate,
                MinSurvivalRate = input.MinSurvivalRate,
                IsMutation = input.IsMutation,
                MinHighQualifiedRate = input.MinHighQualifiedRate,
                PotentialParents = allParents
            };

            var result = await _advisorService.RecommendPairsAsync(request);

            var updatedPairs = new List<BreedingPairResult>();

            foreach (var pair in result.RecommendedPairs)
            {
                try
                {
                    double Fx = await _service.GetOffspringInbreedingAsync(pair.MaleId, pair.FemaleId);

                    Fx = Math.Clamp(Fx, 0.0, 1.0);

                    pair.PercentInbreeding = Math.Round(Fx * 100, 2);
                }
                catch (Exception ex)
                {
                    pair.PercentInbreeding = -1;
                    Console.WriteLine($"❌ Lỗi tính cận huyết cho cặp {pair.MaleId}-{pair.FemaleId}: {ex.Message}");
                }

                updatedPairs.Add(pair);
            }

            // Gán lại vào kết quả gốc
            result.RecommendedPairs = updatedPairs;

            return GetSuccess(result);
        }

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

