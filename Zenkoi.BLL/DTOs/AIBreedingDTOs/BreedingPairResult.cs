using System;
using System.Text.Json.Serialization;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingPairResult
    {
        public int MaleId { get; set; }
        public string MaleRFID { get; set; }
        public string MaleImage { get; set; }

        public bool MaleIsMutated { get; set; }

        public MutationType MaleMutationType { get; set; } = MutationType.None;
        public double? MaleMutationRate { get; set; }

        public int FemaleId { get; set; }
        public string FemaleRFID { get; set; }
        public string FemaleImage { get; set; }

        public bool FemaleIsMutated { get; set; }

        public MutationType FemaleMutationType { get; set; } = MutationType.None;

        public double? FemaleMutationRate { get; set; }

        public string Reason { get; set; } = "";
        public double PredictedFertilizationRate { get; set; }
        public double PredictedHatchRate { get; set; } 
        public double PredictedMutationRate { get; set; }
        

        public double PredictedSurvivalRate { get; set; }
        public double PredictedHighQualifiedRate { get; set; }

        public double PercentInbreeding { get; set; }

        public int Rank { get; set; }
    }
}
