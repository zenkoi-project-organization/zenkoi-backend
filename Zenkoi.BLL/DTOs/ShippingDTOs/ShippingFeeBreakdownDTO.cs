namespace Zenkoi.BLL.DTOs.ShippingDTOs
{
    public class ShippingFeeBreakdownDTO
    {
        public int BoxFee { get; set; }
        public int? ShippingBoxId { get; set; }
        public string? ShippingBoxName { get; set; }

        public List<BoxSelection>? Boxes { get; set; }

        public int DistanceFee { get; set; }
        public int? ShippingDistanceId { get; set; }
        public decimal? DistanceKm { get; set; }

        public int TotalShippingFee { get; set; }

        public string? Notes { get; set; }
    }
}
