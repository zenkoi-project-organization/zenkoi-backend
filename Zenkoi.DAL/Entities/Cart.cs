using System;
using System.Collections.Generic;

namespace Zenkoi.DAL.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}

