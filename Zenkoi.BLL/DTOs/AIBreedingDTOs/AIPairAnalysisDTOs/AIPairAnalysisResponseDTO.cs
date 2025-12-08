using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs
{
    public class AIPairAnalysisResponseDTO
    {
        public int MaleId { get; set; }
        public int FemaleId { get; set; }
        public double PredictedFertilizationRate { get; set; }
        public double PredictedHatchRate { get; set; }
        public double PredictedSurvivalRate { get; set; }
        public double PredictedHighQualifiedRate { get; set; }
        public double PercentInbreeding { get; set; }
        public string? MutationDescription { get; set; }
        public string Summary { get; set; }

        public BreedingInfo MaleBreedingInfo { get; set; }
        public BreedingInfo FemaleBreedingInfo { get; set; }
    }

    public class BreedingInfo
    {
        public string Summary { get; set; }
        public double BreedingSuccessRate { get; set; }
        public double AvgFertilizationRate { get; set; }
        public int? AvgEggs { get; set; }
    }
}
