using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.VarietyDTOs
{
    public class VarietyRequestDTO
    {
        [Required(ErrorMessage = "Tên giống không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên giống không được vượt quá 100 ký tự.")]
        public string VarietyName { get; set; }

        [MaxLength(500, ErrorMessage = "Đặc điểm giống không được vượt quá 500 ký tự.")]
        public string Characteristic { get; set; }

        [MaxLength(100, ErrorMessage = "Tên quốc gia xuất xứ không được vượt quá 100 ký tự.")]
        public string OriginCountry { get; set; }
    }
}
