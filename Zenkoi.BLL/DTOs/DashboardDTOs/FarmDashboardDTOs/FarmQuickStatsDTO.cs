namespace Zenkoi.BLL.DTOs.DashboardDTOs.FarmDashboardDTOs
{
    public class FarmQuickStatsDTO
    {
        public double HealthyKoiPercent { get; set; }
        public ActivePondsDTO ActivePonds { get; set; } = new();
        public ActiveStaffDTO ActiveStaff { get; set; } = new();
        public int ActiveBreedingProcesses { get; set; }
    }

    public class ActivePondsDTO
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }

    public class ActiveStaffDTO
    {
        public int Current { get; set; }
        public int Total { get; set; }
    }
}

