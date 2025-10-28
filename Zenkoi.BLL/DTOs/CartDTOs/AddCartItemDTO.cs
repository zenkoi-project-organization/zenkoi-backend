using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CartDTOs
{
    public class AddCartItemDTO
    {
        [Required]
        public int CustomerId { get; set; }
        
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
}

