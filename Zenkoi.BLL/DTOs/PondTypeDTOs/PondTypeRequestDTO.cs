using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondTypeDTOs
{
    public class PondTypeRequestDTO
    {
        [Required(ErrorMessage = "Tên loại hồ không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên loại hồ không được vượt quá 100 ký tự.")]
        public string TypeName { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Kiểu hồ không được để trống.")]
        public TypeOfPond Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng khuyến nghị phải ≥ 0.")]
        public int? RecommendedQuantity { get; set; }
    }
}