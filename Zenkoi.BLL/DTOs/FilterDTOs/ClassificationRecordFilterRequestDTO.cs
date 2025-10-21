namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class ClassificationRecordFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? ClassificationStageId { get; set; }
        public int? MinStageNumber { get; set; }
        public int? MaxStageNumber { get; set; }
        public int? MinHighQualifiedCount { get; set; }
        public int? MaxHighQualifiedCount { get; set; }
        public int? MinQualifiedCount { get; set; }
        public int? MaxQualifiedCount { get; set; }
        public int? MinUnqualifiedCount { get; set; }
        public int? MaxUnqualifiedCount { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
