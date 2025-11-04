using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CustomerAddressDTOs
{
    public class CustomerAddressRequestDTO
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Full address is required")]
        [StringLength(500, ErrorMessage = "Full address cannot exceed 500 characters")]
        public string FullAddress { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "District cannot exceed 100 characters")]
        public string? District { get; set; }

        [StringLength(100, ErrorMessage = "Ward cannot exceed 100 characters")]
        public string? Ward { get; set; }

        [StringLength(250, ErrorMessage = "Street address cannot exceed 250 characters")]
        public string? StreetAddress { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal? Longitude { get; set; }

        [StringLength(20, ErrorMessage = "Recipient phone cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? RecipientPhone { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
