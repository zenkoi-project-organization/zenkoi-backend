using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class PacketFish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public FishSize Size { get; set; }
        public decimal AgeMonths { get; set; }
        public string? Images { get; set; }
        public string? Video { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<VarietyPacketFish> VarietyPacketFishes { get; set; }
        public ICollection<PondPacketFish> PondPacketFishes { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
