using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordRequestDTO
    {
        public int ClassificationStageId { get; set; }
        public int? CullQualifiedCount { get; set; }
        public string Notes { get; set; }
    }
}
