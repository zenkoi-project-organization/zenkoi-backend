using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class BreedingProcessFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? MaleKoiId { get; set; }
        public int? FemaleKoiId { get; set; }
        public int? PondId { get; set; }
        public BreedingStatus? Status { get; set; }
        public BreedingResult? Result { get; set; }
        public int? MinTotalFishQualified { get; set; }
        public int? MaxTotalFishQualified { get; set; }
        public int? MinTotalPackage { get; set; }
        public int? MaxTotalPackage { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
