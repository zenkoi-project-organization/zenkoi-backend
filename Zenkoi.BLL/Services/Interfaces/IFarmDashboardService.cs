using Zenkoi.BLL.DTOs.DashboardDTOs.FarmDashboardDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IFarmDashboardService
    {
        Task<FarmStatisticsDTO> GetStatisticsAsync();
        Task<List<ActivityFeedDTO>> GetActivityFeedAsync(int limit = 5);
        Task<FarmQuickStatsDTO> GetQuickStatsAsync();
    }
}

