using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zenkoi.DAL.Entities
{
    public class ShippingBox
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WeightCapacityLb { get; set; }
        public decimal Fee { get; set; }
        public int? MaxKoiCount { get; set; }

        public int? MaxKoiSizeInch { get; set; }

        public string Notes { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ShippingBoxRule> Rules { get; set; }
    }
}