using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs
{
     public class WaterParameterThresholdRequestDTO
    {
        [Required(ErrorMessage = "Tên tham số môi trường nước không được để trống.")]
        public WaterParameterType ParameterName { get; set; }

        [Required(ErrorMessage = "Đơn vị đo không được để trống.")]
        [MaxLength(50, ErrorMessage = "Đơn vị đo không được vượt quá 50 ký tự.")]
        public string Unit { get; set; } = default!;

        [Required(ErrorMessage = "vui lòng nhập giá trị tối thiểu ")]
        public double MinValue { get; set; }

        [Required(ErrorMessage = "vui lòng nhập giá trị tối đa.")]
        public double MaxValue { get; set; }
        [Required(ErrorMessage = "vui lòng nhập loại hồ.")]
        public int? PondTypeId { get; set; }
    }
}