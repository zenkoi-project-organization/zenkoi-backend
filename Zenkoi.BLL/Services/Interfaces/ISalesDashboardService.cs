using Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface ISalesDashboardService
    {
        Task<SalesStatisticsDTO> GetStatisticsAsync();
        Task<SalesQuickInfoDTO> GetQuickInfoAsync();
        Task<List<BestSellerDTO>> GetBestSellersAsync(int top = 5);
        Task<SalesAnalysisDTO> GetSalesAnalysisAsync(SalesAnalysisRange range = SalesAnalysisRange.Last30Days);
    }
}

