using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PromotionDTOs
{
    public class PromotionResponseDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Images { get; set; }
    }
}
