using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishUpdateRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn hồ")]
        public int PondId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại.")]
        public int VarietyId { get; set; }

        public int? BreedingProcessId { get; set; }

        [Required(ErrorMessage = "RFID không được bỏ trống.")]
        [MaxLength(50, ErrorMessage = "RFID không quá 50 kí tự.")]
        public string RFID { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Size thì phải >= 0.")]
        public double? Size { get; set; }

        [Required(ErrorMessage = "vui lòng chọn Type.")]
        public KoiType Type { get; set; }

        [Required(ErrorMessage = "Họa tiết thì không được bỏ trống.")]
        public string? Pattern { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Giới tình cá không được bỏ trống")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Sức khỏe cá thì không được bỏ trống.")]
        public HealthStatus HealthStatus { get; set; }

        [Required(ErrorMessage = "SaleStatus thì không được bỏ trống.")]
        public SaleStatus SaleStatus { get; set; } = SaleStatus.NotForSale;

        [MaxLength(100, ErrorMessage = "Origin không được vượt quá 100 kí tụ.")]
        public string? Origin { get; set; }

        public List<string>? Images { get; set; } = new();
        public List<string>? Videos { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "Giá bán phải >= 0")]
        public decimal? SellingPrice { get; set; }

        [MaxLength(1500, ErrorMessage = "Chú thích không vượt quá 1500 kí tự.")]
        public string? Description { get; set; }

        public bool? IsMutated { get; set; }

        public string? MutationDescription { get; set; }
    }
}
