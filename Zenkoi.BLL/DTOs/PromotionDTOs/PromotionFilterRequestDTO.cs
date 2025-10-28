using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PromotionDTOs
{
    public class PromotionFilterRequestDTO
    {
        public string? Search { get; set; } 
        public bool? IsActive { get; set; }
        public DiscountType? DiscountType { get; set; }
        public DateTime? AvailableOnDate { get; set; }
    }
}
