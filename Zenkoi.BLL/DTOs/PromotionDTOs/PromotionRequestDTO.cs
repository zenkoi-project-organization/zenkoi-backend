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
        [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã khuyến mãi không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime ValidTo { get; set; }

        [Required(ErrorMessage = "Loại giảm giá là bắt buộc")]
        public DiscountType DiscountType { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá trị giảm giá phải lớn hơn hoặc bằng 0")]
        public decimal DiscountValue { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal MinimumOrderAmount { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm tối đa phải lớn hơn hoặc bằng 0")]
        public decimal? MaxDiscountAmount { get; set; }

        public int? UsageLimit { get; set; }

        [Required(ErrorMessage = "Trạng thái hoạt động là bắt buộc")]
        public bool IsActive { get; set; }

        public List<string>? Images { get; set; }
    }
}
