namespace Zenkoi.BLL.DTOs.ShippingDistanceDTOs
{
    public class ShippingDistanceRequestDTO
    {
        public string Name { get; set; }
        public int MinDistanceKm { get; set; }
        public int MaxDistanceKm { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal BaseFee { get; set; }
        public string Description { get; set; }
    }
}
