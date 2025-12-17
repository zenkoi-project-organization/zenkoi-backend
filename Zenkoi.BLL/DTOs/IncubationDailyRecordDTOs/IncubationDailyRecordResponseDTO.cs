using Zenkoi.BLL.DTOs.EggBatchDTOs;

namespace Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs
{
    public class IncubationDailyRecordResponseDTO
    {
        public int Id { get; set; }
        public int EggBatchId { get; set; }
        public DateTime DayNumber { get; set; }
        public int? HealthyEggs { get; set; }
        public int? RottenEggs { get; set; }
        public int? HatchedEggs { get; set; }
        public bool Success { get; set; }
    }
}
