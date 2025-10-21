using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class EggBatchFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? BreedingProcessId { get; set; }
        public int? PondId { get; set; }
        public EggBatchStatus? Status { get; set; }
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public double? MinFertilizationRate { get; set; }
        public double? MaxFertilizationRate { get; set; }
        public DateTime? SpawnDateFrom { get; set; }
        public DateTime? SpawnDateTo { get; set; }
        public DateTime? HatchingTimeFrom { get; set; }
        public DateTime? HatchingTimeTo { get; set; }
    }
}
