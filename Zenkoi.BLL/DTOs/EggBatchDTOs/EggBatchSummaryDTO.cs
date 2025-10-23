using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.EggBatchDTOs
{
    public class EggBatchSummaryDTO
    {
        public int EggBatchId { get; set; }
        public double? FertilizationRate { get; set; }
        public int? HealthyEggs { get; set; }
        public int TotalRottenEggs { get; set; }
        public int TotalHatchedEggs { get; set; }
    }
}
