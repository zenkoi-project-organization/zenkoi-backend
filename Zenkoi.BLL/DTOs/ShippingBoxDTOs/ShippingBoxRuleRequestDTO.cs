using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRuleRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ShippingBoxId must be greater than 0")]
        public int ShippingBoxId { get; set; }

        [Required]
        public ShippingRuleType RuleType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxCount must be greater than 0 if specified")]
        public int? MaxCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxLengthCm must be greater than 0 if specified")]
        public int? MaxLengthCm { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MinLengthCm must be greater than 0 if specified")]
        public int? MinLengthCm { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxWeightLb must be greater than 0 if specified")]
        public int? MaxWeightLb { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "ExtraInfo cannot exceed 500 characters")]
        public string ExtraInfo { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Priority must be greater than 0")]
        public int Priority { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}
