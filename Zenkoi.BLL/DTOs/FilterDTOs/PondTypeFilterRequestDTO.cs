namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class PondTypeFilterRequestDTO
    {
        public string? Search { get; set; }
        public int? MinRecommendedCapacity { get; set; }
        public int? MaxRecommendedCapacity { get; set; }
    }
}
