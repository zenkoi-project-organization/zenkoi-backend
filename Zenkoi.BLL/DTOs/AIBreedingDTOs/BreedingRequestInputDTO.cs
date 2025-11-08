using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingRequestInputDTO
    {
        public string TargetVariety { get; set; } = string.Empty;
        public string Priority { get; set; } = "Quality";
        public MutationType DesiredMutationType { get; set; } = MutationType.None;
        public double DesiredMutationRate { get; set; }           
        public double MinHatchRate { get; set; } = 0;             
        public double MinSurvivalRate { get; set; } = 0;         
        public double MinHighQualifiedRate { get; set; } = 0;
    }
}
