using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRequestDTO
    {
        [Required(ErrorMessage = "Tên hộp vận chuyển là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên hộp không được vượt quá 200 ký tự")]
        public string Name { get; set; }
   
        [Range(1, 1000, ErrorMessage = "Trọng tải phải từ 1-1000 lbs")]
        public int WeightCapacityLb { get; set; }

        [Required(ErrorMessage = "Phí vận chuyển là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Phí vận chuyển không được âm (0 = Free Ship)")]
        public decimal Fee { get; set; }

        [Range(1, 100, ErrorMessage = "Sức chứa phải từ 1-100 con cá")]
        public int? MaxKoiCount { get; set; }

        [Range(1, 100, ErrorMessage = "Kích thước tối đa phải từ 1-100 inch")]
        public int? MaxKoiSizeInch { get; set; }

        [Required(ErrorMessage = "Ghi chú là bắt buộc")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Notes { get; set; }
    }
}
