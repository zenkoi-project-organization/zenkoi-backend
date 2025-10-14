using System;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}
