using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingRecordDTO
    {
        public int BreedingProcessId { get; set; }
        public double? FertilizationRate { get; set; }
        public double? HatchRate { get; set; }
        public double? SurvivalRate { get; set; }
        public double? HighQualifiedRate { get; set; }
        public string? ColorPattern { get; set; }
        public string? PartnerVariety { get; set; }
        public string? ResultNote { get; set; }
    }
}
