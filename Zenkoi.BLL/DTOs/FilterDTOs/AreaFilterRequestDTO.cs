namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class AreaFilterRequestDTO
    {
        public string? Search { get; set; }
        public double? MinTotalAreaSQM { get; set; }
        public double? MaxTotalAreaSQM { get; set; }
    }
}
