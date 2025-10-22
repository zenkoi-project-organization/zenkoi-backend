using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingPairResult
    {
        public int MaleId { get; set; }
        public int FemaleId { get; set; }
        public string Reason { get; set; } = "";
        public double PredictedFertilizationRate { get; set; }
        public double PredictedHatchRate { get; set; }
        public double PredictedSurvivalRate { get; set; }
        public double PredictedHighQualifiedRate { get; set; }
        public double PatternMatchScore { get; set; } 
        public double PercentInbreeding { get; set; }
        public int Rank { get; set; }             
    }
}
