using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondDTOs
{
    public class PondUpdateRequestDTO
    {
        [Required(ErrorMessage = "vui lòng chọn loại hồ.")]
        public int PondTypeId { get; set; }

        [Required(ErrorMessage = "vui lòng chọn khu vực.")]
        public int AreaId { get; set; }

        [Required(ErrorMessage = "Tên hồ không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên hồ không được vượt quá 100 ký tự.")]
        public string PondName { get; set; }

        public string? Location { get; set; }

        public PondStatus PondStatus { get; set; } = PondStatus.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Sức chứa hiện tại phải ≥ 0.")]
        public double? CurrentCapacity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Độ sâu phải ≥ 0.")]
        public double? DepthMeters { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chiều dài phải ≥ 0.")]
        public double? LengthMeters { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Chiều rộng phải ≥ 0.")]
        public double? WidthMeters { get; set; }

        public WaterRecordDTO? record { get; set; }

    }
}
