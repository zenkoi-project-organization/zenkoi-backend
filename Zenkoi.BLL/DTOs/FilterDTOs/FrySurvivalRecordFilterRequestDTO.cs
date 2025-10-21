namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class FrySurvivalRecordFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? FryFishId { get; set; }
        public int? MinDayNumber { get; set; }
        public int? MaxDayNumber { get; set; }
        public double? MinSurvivalRate { get; set; }
        public double? MaxSurvivalRate { get; set; }
        public int? MinCountAlive { get; set; }
        public int? MaxCountAlive { get; set; }
        public bool? Success { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
