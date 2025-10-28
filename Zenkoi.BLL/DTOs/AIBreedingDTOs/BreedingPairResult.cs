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
        public string MaleRFID { get; set; }
        public string MaleImage { get; set; }

        public int FemaleId { get; set; }
        public string FemaleRFID { get; set; }
        public string FemaleImage { get; set; }
        public string Reason { get; set; } = "";
        public double PredictedFertilizationRate { get; set; }
        public double PredictedHatchRate { get; set; }
        public double PredictedSurvivalRate { get; set; }
        public double PredictedHighQualifiedRate { get; set; }
        public double PatternMatchScore { get; set; }     
        public double BodyShapeCompatibility { get; set; }
        public string? PercentInbreeding { get; set; } = "unknown";

        public int Rank { get; set; }

        // 👉 Thêm helper property nếu cần dùng dưới dạng số
        public double? PercentInbreedingValue =>
            double.TryParse(PercentInbreeding, out var value) ? value : null;
    }
}
