using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public ApplicationUser? ApplicationUser { get; set; }
        public string? ContactNumber { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();
        public Cart? Cart { get; set; }
    }
}
