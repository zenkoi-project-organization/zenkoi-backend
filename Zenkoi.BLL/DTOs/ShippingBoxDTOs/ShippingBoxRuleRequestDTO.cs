using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRuleRequestDTO
    {
        public int ShippingBoxId { get; set; }
        public ShippingRuleType RuleType { get; set; }
        public int? MaxCount { get; set; }
        public int? MaxLengthCm { get; set; }
        public int? MinLengthCm { get; set; }
        public int? MaxWeightLb { get; set; }
        public string ExtraInfo { get; set; }
        public int Priority { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }
}
