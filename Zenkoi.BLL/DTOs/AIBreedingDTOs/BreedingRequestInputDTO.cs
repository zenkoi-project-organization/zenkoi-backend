using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingRequestInputDTO
    {
        public string TargetVariety { get; set; } = "";
        public string Priority { get; set; } = ""; // "Quality", "Quantity", or "Both"
        public string DesiredPattern { get; set; } = "";
        public string DesiredBodyShape { get; set; } = "";

        public double MinHatchRate { get; set; }
        public double MinSurvivalRate { get; set; }
        public double MinHighQualifiedRate { get; set; }
    }
}
