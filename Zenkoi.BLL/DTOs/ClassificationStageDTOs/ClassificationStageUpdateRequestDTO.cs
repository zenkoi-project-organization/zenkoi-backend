using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationStageDTOs
{
    public class ClassificationStageUpdateRequestDTO
    {
        [Required(ErrorMessage = "vui lòng chọn hồ")]
        public int PondId { get; set; }
        [MaxLength(500, ErrorMessage = "Notes tối đa 500 ký tự")]
        public string? Notes { get; set; }
    }
}
