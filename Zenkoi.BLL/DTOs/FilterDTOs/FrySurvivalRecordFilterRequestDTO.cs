namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class FrySurvivalRecordFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? FryFishId { get; set; }
        public DateTime? MinDayNumber { get; set; }
        public DateTime? MaxDayNumber { get; set; }
        public double? MinSurvivalRate { get; set; }
        public double? MaxSurvivalRate { get; set; }
        public int? MinCountAlive { get; set; }
        public int? MaxCountAlive { get; set; }
        public bool? Success { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
