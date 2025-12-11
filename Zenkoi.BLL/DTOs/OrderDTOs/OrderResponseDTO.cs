using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.BLL.DTOs.PromotionDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.OrderDTOs
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Note { get; set; }
        public int? PromotionId { get; set; }
        public PromotionResponseDTO? Promotion { get; set; }
        public List<OrderDetailResponseDTO> OrderDetails { get; set; } = new List<OrderDetailResponseDTO>();
    }

    public class OrderDetailResponseDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? KoiFishId { get; set; }
        public int? PacketFishId { get; set; }     
        public KoiFishResponseDTO? KoiFish { get; set; }
        public PacketFishResponseDTO? PacketFish { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
