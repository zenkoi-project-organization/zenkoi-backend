using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiIncidentController : BaseAPIController
    {
        private readonly IIncidentService _incidentService;

        public KoiIncidentController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử sức khỏe của cá Koi
        /// </summary>
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
