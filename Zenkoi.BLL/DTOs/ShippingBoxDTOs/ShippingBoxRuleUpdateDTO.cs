using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRuleUpdateDTO
    {
        [Required(ErrorMessage = "Loại quy tắc là bắt buộc")]
        public ShippingRuleType RuleType { get; set; }

        [Range(1, 100, ErrorMessage = "Số lượng tối đa phải từ 1-100 nếu có")]
        public int? MaxCount { get; set; }

        [Range(1, 200, ErrorMessage = "Chiều dài tối đa phải từ 1-200cm nếu có")]
        public int? MaxLengthCm { get; set; }

        [Range(1, 200, ErrorMessage = "Chiều dài tối thiểu phải từ 1-200cm nếu có")]
        public int? MinLengthCm { get; set; }

        [Range(1, 1000, ErrorMessage = "Trọng lượng tối đa phải từ 1-1000 lbs nếu có")]
        public int? MaxWeightLb { get; set; }

        [Required(ErrorMessage = "Thông tin bổ sung là bắt buộc")]
        [StringLength(500, ErrorMessage = "Thông tin bổ sung không được vượt quá 500 ký tự")]
        public string ExtraInfo { get; set; }

        [Required(ErrorMessage = "Độ ưu tiên là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Độ ưu tiên phải từ 1-100")]
        public int Priority { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }
}
