using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRequestDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "WeightCapacityLb must be greater than 0")]
        public int WeightCapacityLb { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fee must be greater than 0")]
        public decimal Fee { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxKoiCount must be greater than 0 if specified")]
        public int? MaxKoiCount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxKoiSizeInch must be greater than 0 if specified")]
        public int? MaxKoiSizeInch { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}
