using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.PatternDTOs
{
    public class PatternRequestDTO
    {
        [Required(ErrorMessage = "Tên họa tiết không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên họa tiết không được vượt quá 100 ký tự.")]
        public string PatternName { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string Description { get; set; }
    }
}
