using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class FryFishFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? BreedingProcessId { get; set; }
        public int? PondId { get; set; }
        public FryFishStatus? Status { get; set; }
        public int? MinInitialCount { get; set; }
        public int? MaxInitialCount { get; set; }
        public double? MinCurrentSurvivalRate { get; set; }
        public double? MaxCurrentSurvivalRate { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
