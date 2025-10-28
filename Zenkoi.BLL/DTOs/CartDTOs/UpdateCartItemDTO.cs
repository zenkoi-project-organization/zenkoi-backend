using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CartDTOs
{
    public class UpdateCartItemDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
}

