using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingRequestDTO
    {
        public string TargetVariety { get; set; } = "";
        public string Priority { get; set; } = ""; // "Quality", "Quantity", or "Both"
        public string DesiredPattern { get; set; } = "";
        public double MinHatchRate { get; set; }
        public double MinSurvivalRate { get; set; }
        public double MinHighQualifiedRate { get; set; }
        public List<BreedingParentDTO> PotentialParents { get; set; } = new();
    }
}
