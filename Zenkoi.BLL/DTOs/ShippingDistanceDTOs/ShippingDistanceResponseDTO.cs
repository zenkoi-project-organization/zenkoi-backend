namespace Zenkoi.BLL.DTOs.ShippingDistanceDTOs
{
    public class ShippingDistanceResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinDistanceKm { get; set; }
        public int MaxDistanceKm { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal BaseFee { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
