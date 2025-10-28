using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.OrderDTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<OrderItemDTO> Items { get; set; } = new List<OrderItemDTO>();

        public decimal ShippingFee { get; set; } = 0;

        public int? PromotionId { get; set; }
    }

    public class OrderItemDTO
    {
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
