using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.DashboardDTOs.FarmDashboardDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("FarmStaff")]
    public class FarmDashboardController : BaseAPIController
    {
        private readonly IFarmDashboardService _farmDashboardService;

        public FarmDashboardController(IFarmDashboardService farmDashboardService)
        {
            _farmDashboardService = farmDashboardService;
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var result = await _farmDashboardService.GetStatisticsAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thống kê: {ex.Message}");
            }
        }

        [HttpGet("activity-feed")]
        public async Task<IActionResult> GetActivityFeed([FromQuery] int limit = 5)
        {
            try
            {
                if (limit <= 0)
                {
                    return GetError("Tham số 'limit' phải là số nguyên dương.");
                }

                var result = await _farmDashboardService.GetActivityFeedAsync(limit);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách hoạt động: {ex.Message}");
            }
        }

        [HttpGet("quick-stats")]
        public async Task<IActionResult> GetQuickStats()
        {
            try
            {
                var result = await _farmDashboardService.GetQuickStatsAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin nhanh: {ex.Message}");
            }
        }
    }
}

