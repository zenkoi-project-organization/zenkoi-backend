using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CartDTOs
{
    public class AddCartItemDTO
    {         
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }
        
        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; } = 1;
    }
}

