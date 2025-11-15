namespace Zenkoi.BLL.DTOs.DashboardDTOs.FarmDashboardDTOs
{
    public class FarmStatisticsDTO
    {
        public TotalKoiDTO TotalKoi { get; set; } = new();
        public ActiveAccountsDTO ActiveAccounts { get; set; } = new();
        public PondsInUseDTO PondsInUse { get; set; } = new();
        public FarmMonthlyRevenueDTO FarmMonthlyRevenue { get; set; } = new();
    }

    public class TotalKoiDTO
    {
        public int Current { get; set; }
        public double ChangePercent { get; set; }
    }

    public class ActiveAccountsDTO
    {
        public int Current { get; set; }
        public double ChangePercent { get; set; }
    }

    public class PondsInUseDTO
    {
        public int Current { get; set; }
        public double ChangePercent { get; set; }
    }

    public class FarmMonthlyRevenueDTO
    {
        public decimal Current { get; set; }
        public double ChangePercent { get; set; }
    }
}

