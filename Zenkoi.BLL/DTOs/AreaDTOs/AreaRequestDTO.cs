using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AreaDTOs
{
    public class AreaRequestDTO
    {
        [Required(ErrorMessage = "AreaName không được để trống")]
        [MaxLength(100, ErrorMessage = "AreaName không được vượt quá 100 ký tự")]
        public string AreaName { get; set; }
        
        [Range(1, 1000000, ErrorMessage = "TotalAreaSQM phải lớn hơn 0")]
        public double? TotalAreaSQM { get; set; }

        [MaxLength(500, ErrorMessage = "Description tối đa 500 ký tự")]
        public string Description { get; set; }
    }
}
