using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Enums;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesDashboardController : BaseAPIController
    {
        private readonly ISalesDashboardService _salesDashboardService;

        public SalesDashboardController(ISalesDashboardService salesDashboardService)
        {
            _salesDashboardService = salesDashboardService;
        }

        /// <summary>
        /// Lấy 4 chỉ số thống kê chính (KPIs) của dashboard bán hàng
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var result = await _salesDashboardService.GetStatisticsAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thống kê: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy thông tin nhanh cho box "Thông tin nhanh"
        /// </summary>
        [HttpGet("quick-info")]
        public async Task<IActionResult> GetQuickInfo()
        {
            try
            {
                var result = await _salesDashboardService.GetQuickInfoAsync();
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy thông tin nhanh: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách "Cá bán chạy nhất"
        /// </summary>
        /// <param name="top">Số lượng top items (mặc định: 5)</param>
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers([FromQuery] int top = 5)
        {
            try
            {
                if (top <= 0)
                {
                    return GetError("Tham số 'top' phải là số nguyên dương.");
                }

                var result = await _salesDashboardService.GetBestSellersAsync(top);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy danh sách cá bán chạy: {ex.Message}");
            }
        }

        [HttpGet("sales-analysis")]
        public async Task<IActionResult> GetSalesAnalysis([FromQuery] SalesAnalysisRange range = SalesAnalysisRange.Last30Days)
        {
            try
            {
                var result = await _salesDashboardService.GetSalesAnalysisAsync(range);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Lỗi khi lấy dữ liệu phân tích bán hàng: {ex.Message}");
            }
        }
       
    }
}

