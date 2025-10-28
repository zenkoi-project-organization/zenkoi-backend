using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordV2RequestDTO
    {
        public int ClassificationStageId { get; set; }
        public int? HighQualifiedCount { get; set; }
        public string Notes { get; set; }
    }
}
