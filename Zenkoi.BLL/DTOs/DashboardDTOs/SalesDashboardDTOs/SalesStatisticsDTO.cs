namespace Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs
{
    public class SalesStatisticsDTO
    {
        public MonthlyRevenueDTO MonthlyRevenue { get; set; } = new();
        public TotalOrdersDTO TotalOrders { get; set; } = new();
        public CustomerCountDTO CustomerCount { get; set; } = new();
        public FishInStockDTO FishInStock { get; set; } = new();
    }

    public class MonthlyRevenueDTO
    {
        public decimal Current { get; set; }
        public double ChangePercent { get; set; }
    }

    public class TotalOrdersDTO
    {
        public int Current { get; set; }
        public double ChangePercent { get; set; }
    }

    public class CustomerCountDTO
    {
        public int Current { get; set; }
        public double NewCustomerPercent { get; set; }
    }

    public class FishInStockDTO
    {
        public int Current { get; set; }
        public int LowStockCount { get; set; }
    }
}

