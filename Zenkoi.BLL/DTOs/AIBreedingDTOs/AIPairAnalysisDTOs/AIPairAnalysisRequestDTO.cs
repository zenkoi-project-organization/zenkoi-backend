using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs
{
    public class AIPairAnalysisRequestDTO
    {
        public BreedingParentDTO Male { get; set; }
        public BreedingParentDTO Female { get; set; }
        public string TargetVariety { get; set; }
        public string DesiredPattern { get; set; }
        public string DesiredBodyShape { get; set; }
    }
}
