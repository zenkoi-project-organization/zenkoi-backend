namespace Zenkoi.BLL.DTOs.CustomerAddressDTOs
{
    public class CustomerAddressResponseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Ward { get; set; }
        public string? StreetAddress { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? DistanceFromFarmKm { get; set; }
        public DateTime? DistanceCalculatedAt { get; set; }
        public string? RecipientPhone { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
