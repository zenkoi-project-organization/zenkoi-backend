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
        public int FishPerPacket { get; set; } = 10;
        public decimal PricePerPacket { get; set; }
        public double MinSize { get; set; }
        public double MaxSize { get; set; }
        public decimal AgeMonths { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<VarietyPacketFish> VarietyPacketFishes { get; set; } = new List<VarietyPacketFish>();
        public ICollection<PondPacketFish> PondPacketFishes { get; set; } = new List<PondPacketFish>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
