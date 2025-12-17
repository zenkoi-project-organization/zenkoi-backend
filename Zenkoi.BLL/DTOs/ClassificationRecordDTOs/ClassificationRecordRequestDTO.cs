using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordRequestDTO
    {
        [Required(ErrorMessage = "ClassificationStageId không được để trống")]
        public int ClassificationStageId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng cá bỏ phải lớn hơn 0")]
        public int? CullQualifiedCount { get; set; }

        [MaxLength(500, ErrorMessage = "Notes tối đa 500 ký tự")]
        public string Notes { get; set; }
    }
}
