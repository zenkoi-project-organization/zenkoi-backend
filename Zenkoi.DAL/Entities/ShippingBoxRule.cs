using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class ShippingBoxRule
    {
        public int Id { get; set; }
        public int ShippingBoxId { get; set; }
        public ShippingBox ShippingBox { get; set; }

        public ShippingRuleType RuleType { get; set; }

        public int? MaxCount { get; set; }

        public int? MaxLengthCm { get; set; }

        public int? MinLengthCm { get; set; }

        public int? MaxWeightLb { get; set; }

        public string ExtraInfo { get; set; }

        public int Priority { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}