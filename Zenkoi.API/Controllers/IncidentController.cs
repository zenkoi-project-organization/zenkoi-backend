using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.IncidentDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Enums;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncidentController : BaseAPIController
    {
        private readonly IIncidentService _incidentService;

        public IncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        [HttpGet]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> GetAll(
            [FromQuery] IncidentFilterDTO? filter,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var data = await _incidentService.GetAllIncidentsAsync(filter ?? new IncidentFilterDTO(), pageIndex, pageSize);
                return GetPagedSuccess(data);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể lấy danh sách sự cố: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var incident = await _incidentService.GetByIdAsync(id);
                return GetSuccess(incident);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể lấy thông tin sự cố: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> Create([FromBody] CreateIncidentWithDetailsDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var incident = await _incidentService.CreateIncidentWithDetailsAsync(UserId, dto);
                return SaveSuccess(incident, "Tạo sự cố kèm chi tiết thành công.");
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
                return GetError($"Không thể tạo sự cố: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> Update(int id, [FromBody] IncidentUpdateRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var incident = await _incidentService.UpdateAsync(id, UserId, dto);
                return Success(incident, "Cập nhật sự cố thành công.");
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
                return GetError($"Không thể cập nhật sự cố: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _incidentService.DeleteAsync(id);
                return Success(success, "Xóa sự cố thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể xóa sự cố: {ex.Message}");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var incident = await _incidentService.ChangeStatusAsync(id, UserId, dto.Status, dto.ResolutionNotes, dto.ResolutionImages);
                return Success(incident, "Thay đổi trạng thái sự cố thành công.");
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
                return GetError($"Không thể thay đổi trạng thái sự cố: {ex.Message}");
            }
        }

        [HttpPost("{id}/koi")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> AddKoiIncident(int id, [FromBody] KoiIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var koiIncident = await _incidentService.AddKoiIncidentAsync(id, dto);
                return SaveSuccess(koiIncident, "Thêm cá Koi vào sự cố thành công.");
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
                return GetError($"Không thể thêm cá Koi vào sự cố: {ex.Message}");
            }
        }

        [HttpPost("{id}/pond")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> AddPondIncident(int id, [FromBody] PondIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var pondIncident = await _incidentService.AddPondIncidentAsync(id, dto);
                return SaveSuccess(pondIncident, "Thêm ao vào sự cố thành công.");
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
                return GetError($"Không thể thêm ao vào sự cố: {ex.Message}");
            }
        }

        [HttpPut("{id}/koi/{koiIncidentId}")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> UpdateKoiIncident(int id, int koiIncidentId, [FromBody] UpdateKoiIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var koiIncident = await _incidentService.UpdateKoiIncidentAsync(koiIncidentId, dto);
                return Success(koiIncident, "Cập nhật thông tin cá Koi thành công.");
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
                return GetError($"Không thể cập nhật thông tin cá Koi: {ex.Message}");
            }
        }

        [HttpPut("{id}/pond/{pondIncidentId}")]
        [Authorize(Roles = "Manager,FarmStaff")]
        public async Task<IActionResult> UpdatePondIncident(int id, int pondIncidentId, [FromBody] UpdatePondIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var pondIncident = await _incidentService.UpdatePondIncidentAsync(pondIncidentId, dto);
                return Success(pondIncident, "Cập nhật thông tin ao thành công.");
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
                return GetError($"Không thể cập nhật thông tin ao: {ex.Message}");
            }
        }

        [HttpGet("koi/{koiFishId}/history")]
        public async Task<IActionResult> GetKoiHealthHistory(int koiFishId)
        {
            try
            {
                var history = await _incidentService.GetKoiHealthHistoryAsync(koiFishId);
                return GetSuccess(history);
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể lấy lịch sử sức khỏe: {ex.Message}");
            }
        }

    }
}
