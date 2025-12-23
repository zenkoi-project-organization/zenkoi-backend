using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.ShippingDistanceDTOs
{
    public class ShippingDistanceRequestDTO
    {
        [Required(ErrorMessage = "Tên khoảng cách là bắt buộc")]
        [StringLength(500, ErrorMessage = "Tên không được vượt quá 500 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Khoảng cách tối thiểu là bắt buộc")]
        [Range(0, 10000, ErrorMessage = "Khoảng cách tối thiểu phải từ 0-10000 km")]
        public int MinDistanceKm { get; set; }

        [Required(ErrorMessage = "Khoảng cách tối đa là bắt buộc")]
        [Range(0, 10000, ErrorMessage = "Khoảng cách tối đa phải từ 0-10000 km")]
        public int MaxDistanceKm { get; set; }

        [Required(ErrorMessage = "Giá theo km là bắt buộc")]
        [Range(0, 1000000, ErrorMessage = "Giá theo km phải từ 0-1,000,000")]
        public decimal PricePerKm { get; set; }

        [Required(ErrorMessage = "Phí cơ bản là bắt buộc")]
        [Range(0, 10000000, ErrorMessage = "Phí cơ bản phải từ 0-10,000,000")]
        public decimal BaseFee { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; }
    }
}
