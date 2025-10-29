using System.ComponentModel.DataAnnotations;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;

namespace Zenkoi.BLL.DTOs.CartDTOs
{
    public class CartResponseDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartItemResponseDTO> CartItems { get; set; } = new List<CartItemResponseDTO>();
        public decimal TotalPrice { get; set; }
    }

    public class ConvertCartToOrderDTO
    {    
        public decimal ShippingFee { get; set; } = 0;
        
        public int? PromotionId { get; set; }
        
    }
}

    public class CartItemResponseDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }

        public KoiFishResponseDTO KoiFish { get; set; }
        public PacketFishResponseDTO PacketFish { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotalPrice { get; set; }
        public DateTime AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

