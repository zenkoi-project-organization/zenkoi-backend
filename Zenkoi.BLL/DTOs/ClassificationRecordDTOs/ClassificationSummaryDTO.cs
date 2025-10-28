using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationRecordDTOs
{
    public class ClassificationSummaryDTO
    {
        public int ClassificationStageId { get; set; }
        public int CurrentFish {  get; set; }
        public int TotalHighQualified { get; set; }
        public int TotalShowQualified { get; set; }
        public int TotalCullQualified { get; set; }
        public int TotalPondQualified { get; set; }
    }
}
