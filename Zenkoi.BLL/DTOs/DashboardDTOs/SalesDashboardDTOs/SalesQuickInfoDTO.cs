namespace Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs
{
    public class SalesQuickInfoDTO
    {
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int BestSellingItemsCount { get; set; }
        public List<string>? Alerts { get; set; }
    }
}

