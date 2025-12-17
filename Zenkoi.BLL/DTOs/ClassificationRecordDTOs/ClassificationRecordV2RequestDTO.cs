using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordV2RequestDTO
    {
        [Required(ErrorMessage = "ClassificationStageId không được để trống")]
        public int ClassificationStageId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Cá chất lượng cao phải ≥ 0")]
        public int? HighQualifiedCount { get; set; }

        [MaxLength(500, ErrorMessage = "Notes tối đa 500 ký tự")]
        public string Notes { get; set; }
    }
}
