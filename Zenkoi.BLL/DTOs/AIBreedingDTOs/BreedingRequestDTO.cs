using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingRequestDTO
    {
        public string TargetVariety { get; set; } = "";
        public string Priority { get; set; } = ""; // "Quality", "Quantity", or "Both"
        public double DesiredMutationRate { get; set; }
        public MutationType DesiredMutationType { get; set; }
        public double MinHatchRate { get; set; }
        public double MinSurvivalRate { get; set; }
        public double MinHighQualifiedRate { get; set; }

      public List<BreedingParentDTO> PotentialParents { get; set; } = new();
    }
}
