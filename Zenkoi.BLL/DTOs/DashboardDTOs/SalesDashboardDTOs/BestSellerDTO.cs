namespace Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs
{
    public class BestSellerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SoldCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

