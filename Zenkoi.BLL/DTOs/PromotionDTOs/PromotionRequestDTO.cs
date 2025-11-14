using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PromotionDTOs
{
    public class PromotionRequestDTO
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }

        [Required]
        public DiscountType DiscountType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }

        public decimal MinimumOrderAmount { get; set; } = 0;

        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public List<string>? Images { get; set; }
    }
}
