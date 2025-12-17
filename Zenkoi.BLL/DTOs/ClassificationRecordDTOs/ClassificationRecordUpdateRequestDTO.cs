using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordUpdateRequestDTO
    {
        [Range(0, int.MaxValue, ErrorMessage = "Cá chất lượng cao phải ≥ 0")]
        public int? HighQualifiedCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Cá trưng bài  phải ≥ 0")]
        public int? ShowQualifiedCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Cá thương phẩm phải ≥ 0")]
        public int? PondQualifiedCount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Cá bỏ phải ≥ 0")]
        public int? CullQualifiedCount { get; set; }

        [MaxLength(500, ErrorMessage = "Notes tối đa 500 ký tự")]
        public string Notes { get; set; }
    }
}
