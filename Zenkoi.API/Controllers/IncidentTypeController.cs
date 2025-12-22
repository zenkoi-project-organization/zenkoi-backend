using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.IncidentTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentTypeController : BaseAPIController
    {
        private readonly IIncidentTypeService _incidentTypeService;

        public IncidentTypeController(IIncidentTypeService incidentTypeService)
        {
            _incidentTypeService = incidentTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllIncidentTypes(
            [FromQuery] IncidentTypeFilterRequestDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _incidentTypeService.GetAllIncidentTypesAsync(
                    filter ?? new IncidentTypeFilterRequestDTO(),
                    pageIndex,
                    pageSize);
                return GetPagedSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể lấy danh sách loại sự cố: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetIncidentTypeById(int id)
        {
            try
            {
                var result = await _incidentTypeService.GetIncidentTypeByIdAsync(id);
                return GetSuccess(result);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể lấy thông tin loại sự cố: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> CreateIncidentType([FromBody] IncidentTypeRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _incidentTypeService.CreateIncidentTypeAsync(dto);
                return SaveSuccess(result, "Tạo loại sự cố thành công.");
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể tạo loại sự cố: {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> UpdateIncidentType(int id, [FromBody] IncidentTypeUpdateRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var result = await _incidentTypeService.UpdateIncidentTypeAsync(id, dto);
                return Success(result, "Cập nhật loại sự cố thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return GetError(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể cập nhật loại sự cố: {ex.Message}");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteIncidentType(int id)
        {
            try
            {
                var success = await _incidentTypeService.DeleteIncidentTypeAsync(id);
                return Success(success, "Xóa loại sự cố thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể xóa loại sự cố: {ex.Message}");
            }
        }
    }
}

