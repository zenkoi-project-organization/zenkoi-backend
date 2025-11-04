using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class PondTypeFilterRequestDTO
    {
        public string? Search { get; set; }
        public TypeOfPond? Type {  get; set; }
        public int? MinRecommendedQuantity { get; set; }
        public int? MaxRecommendedQuantity { get; set; }
    }
}
