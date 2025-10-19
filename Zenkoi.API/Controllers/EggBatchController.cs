using Microsoft.AspNetCore.Mvc;
using Zenkoi.API;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.Services.Interfaces;

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
            try
            {
                var result = await _eggBatchService.GetAllEggBatchAsync(pageIndex, pageSize);

                var response = new
                {
                    result.PageIndex,
                    result.TotalPages,
                    result.TotalItems,
                    result.HasNextPage,
                    result.HasPreviousPage,
                    Data = result
                };

                return GetSuccess(response);
            }
        catch (KeyNotFoundException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
            {
                return GetError($"Get egg batches failed: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _eggBatchService.GetByIdAsync(id);
                if (result == null)
                    return GetNotFound("Không tìm thấy lô trứng");

                return GetSuccess(result);
            }
        catch (KeyNotFoundException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
            {
                return GetError($"Get egg batch failed: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EggBatchRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var result = await _eggBatchService.CreateAsync(dto);
                return SaveSuccess(result);
            }
        catch (KeyNotFoundException ex)
        {
            return GetError(ex.Message);
        }
        catch (Exception ex)
            {
                return SaveError(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _eggBatchService.DeleteAsync(id);
                if (!deleted)
                    return GetNotFound("Không tìm thấy lô trứng để xóa");

                return Success(null, "Xóa lô trứng thành công");
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Delete egg batch failed: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EggBatchUpdateRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            try
            {
                var updated = await _eggBatchService.UpdateAsync(id, dto);
     
                return SaveSuccess(null);
            }
            catch (KeyNotFoundException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Update egg batch failed: {ex.Message}");
            }
        }
    }
