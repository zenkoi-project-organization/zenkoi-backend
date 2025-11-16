namespace Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs
{
    public class SalesAnalysisDTO
    {
        public List<string> Labels { get; set; } = new();
        public List<decimal> RevenueData { get; set; } = new();
        public List<int> OrdersData { get; set; } = new();
    }
}

