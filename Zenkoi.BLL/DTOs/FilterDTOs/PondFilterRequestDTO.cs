using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class PondFilterRequestDTO
    {
        public string? Search { get; set; }
        public PondStatus? Status { get; set; }
        public int? AreaId { get; set; }
        public int? PondTypeId { get; set; }
        public TypeOfPond? PondTypeEnum { get; set; }
        public bool? Available { get; set; }
        public double? MinCapacityLiters { get; set; }
        public double? MaxCapacityLiters { get; set; }
        public double? MinDepthMeters { get; set; }
        public double? MaxDepthMeters { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}
