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
        public async Task<IActionResult> Create([FromBody] IncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                int userId = UserId;
                var incident = await _incidentService.CreateAsync(userId, dto);
                return SaveSuccess(incident, "Tạo sự cố thành công.");
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
        public async Task<IActionResult> Update(int id, [FromBody] IncidentUpdateRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var incident = await _incidentService.UpdateAsync(id, dto);
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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var success = await _incidentService.ChangeStatusAsync(id, dto);
                return Success(success, "Thay đổi trạng thái sự cố thành công.");
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

        [HttpPut("{id}/resolve")]
        public async Task<IActionResult> Resolve(int id, [FromBody] ResolveIncidentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var success = await _incidentService.ResolveAsync(id, UserId, dto);
                return Success(success, "Giải quyết sự cố thành công.");
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
                return GetError($"Không thể giải quyết sự cố: {ex.Message}");
            }
        }

        [HttpPost("{id}/koi")]
        public async Task<IActionResult> AddKoiIncident(int id, [FromBody] KoiIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var koiIncident = await _incidentService.AddKoiIncidentAsync(id, dto);
                return SaveSuccess(koiIncident, "Thêm cá bị ảnh hưởng thành công.");
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
                return GetError($"Không thể thêm cá bị ảnh hưởng: {ex.Message}");
            }
        }

        [HttpDelete("koi/{koiIncidentId}")]
        public async Task<IActionResult> RemoveKoiIncident(int koiIncidentId)
        {
            try
            {
                var success = await _incidentService.RemoveKoiIncidentAsync(koiIncidentId);
                return Success(success, "Xóa cá bị ảnh hưởng thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể xóa cá bị ảnh hưởng: {ex.Message}");
            }
        }

        [HttpPost("{id}/pond")]
        public async Task<IActionResult> AddPondIncident(int id, [FromBody] PondIncidentRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ModelInvalid();

                var pondIncident = await _incidentService.AddPondIncidentAsync(id, dto);
                return SaveSuccess(pondIncident, "Thêm ao bị ảnh hưởng thành công.");
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
                return GetError($"Không thể thêm ao bị ảnh hưởng: {ex.Message}");
            }
        }

        [HttpDelete("pond/{pondIncidentId}")]
        public async Task<IActionResult> RemovePondIncident(int pondIncidentId)
        {
            try
            {
                var success = await _incidentService.RemovePondIncidentAsync(pondIncidentId);
                return Success(success, "Xóa ao bị ảnh hưởng thành công.");
            }
            catch (KeyNotFoundException ex)
            {
                return GetNotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể xóa ao bị ảnh hưởng: {ex.Message}");
            }
        }
    }
}
