using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationRecordV1RequestDTO
    {
        public int ClassificationStageId { get; set; }
        public int? CullQualifiedCount { get; set; }
        public double? MutatedFishCount { get; set; }
        public string Notes { get; set; }
    }
}
