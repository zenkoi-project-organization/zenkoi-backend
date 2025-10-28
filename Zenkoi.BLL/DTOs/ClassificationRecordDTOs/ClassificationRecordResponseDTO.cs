using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordResponseDTO
    {
        public int Id { get; set; }
        public int ClassificationStageId { get; set; }

        public string StageNumber { get; set; }
        public int? HighQualifiedCount { get; set; }
        public int? ShowQualifiedCount { get; set; }
        public int? PondQualifiedCount { get; set; }
        public int? CullQualifiedCount { get; set; }
        public string Notes { get; set; }
    }
}
