using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost]
        public async Task<ActionResult> CreateBreeding([FromBody] BreedingProcessRequestDTO dto)
        {
            try
            {
                var breeding = await _service.AddBreeding(dto);
                return GetSuccess(breeding);

            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Tính hệ số cận huyết của con khi phối giống giữa cá đực và cá cái.
        /// </summary>
        [HttpGet("offspring")]
        public async Task<ActionResult> GetOffspringInbreeding(
            [FromQuery] int maleId,
            [FromQuery] int femaleId)
        {
            if (maleId <= 0 || femaleId <= 0)
                return BadRequest("maleId và femaleId phải là số nguyên dương.");

            var result = await _service.GetOffspringInbreedingAsync(maleId, femaleId);

            return GetSuccess(new
            {
                MaleId = maleId,
                FemaleId = femaleId,
                InbreedingCoefficient = result
            });
        }

        /// <summary>
        /// Tính hệ số cận huyết của một cá thể (nếu có đủ thông tin bố mẹ).
        /// </summary>
        [HttpGet("individual/{koiId:int}")]
        public async Task<IActionResult> GetIndividualInbreeding([FromRoute] int koiId)
        {
          

            var result = await _service.GetIndividualInbreedingAsync(koiId);

            return GetSuccess(new
            {
                KoiId = koiId,
                InbreedingCoefficient = result
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBreedingProcesses(
          [FromQuery] int pageIndex = 1,
          [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedList = await _service.GetAllBreedingProcess(pageIndex, pageSize);

                return GetSuccess(new
                {
                    paginatedList.PageIndex,
                    paginatedList.TotalPages,
                    paginatedList.TotalItems,
                    paginatedList.HasPreviousPage,
                    paginatedList.HasNextPage,
                    Data = paginatedList
                });
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// Lấy chi tiết một quy trình sinh sản theo Id
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBreedingById(int id)
        {
            try
            {
                var breeding = await _service.GetBreedingById(id);
                return GetSuccess(breeding);
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}
