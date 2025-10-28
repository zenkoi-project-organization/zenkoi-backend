using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class Customer
    {
        public int Id { get; set; }   
        public int ApplicationUserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ContactNumber { get; set; }   
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public Cart? Cart { get; set; }
    }
}
