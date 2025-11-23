using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = Guid.NewGuid().ToString("N").ToUpper();

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? CustomerAddressId { get; set; }
        public CustomerAddress? CustomerAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public int? PromotionId { get; set; }
        public Promotion? Promotion { get; set; }

        public Payment? Payment { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
