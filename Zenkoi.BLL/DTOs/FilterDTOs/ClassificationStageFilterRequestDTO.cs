using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class ClassificationStageFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? BreedingProcessId { get; set; }
        public int? PondId { get; set; }
        public ClassificationStatus? Status { get; set; }
        public int? MinTotalCount { get; set; }
        public int? MaxTotalCount { get; set; }
        public int? MinHighQualifiedCount { get; set; }
        public int? MaxHighQualifiedCount { get; set; }
        public int? MinQualifiedCount { get; set; }
        public int? MaxQualifiedCount { get; set; }
        public int? MinUnqualifiedCount { get; set; }
        public int? MaxUnqualifiedCount { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
