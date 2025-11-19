using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class PacketFishFilterRequestDTO
    {
        public string? Search { get; set; }
        public bool? IsAvailable { get; set; }
        public double? MinSize { get; set; }
        public double? MaxSize { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinAgeMonths { get; set; }
        public decimal? MaxAgeMonths { get; set; }
        public int? MinStockQuantity { get; set; }
        public int? MaxStockQuantity { get; set; }
        public List<int>? VarietyIds { get; set; }
    }
}