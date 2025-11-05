using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.ShippingDTOs
{
    public class CalculateShippingFeeRequestDTO
    {
        [Required]
        public List<ShippingItemDTO> Items { get; set; } = new List<ShippingItemDTO>();

        [Required]
        public int CustomerAddressId { get; set; }
    }

    public class ShippingItemDTO
    {
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
