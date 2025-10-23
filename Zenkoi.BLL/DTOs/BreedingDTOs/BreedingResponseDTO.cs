using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.BreedingDTOs
{
    public class BreedingResponseDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int MaleKoiId { get; set; }
        public string? MaleKoiRFID { get; set; }
        public string? MaleKoiVariety { get; set; }

        public int FemaleKoiId { get; set; }

        public string? FemaleKoiRFID { get; set; }
        public string? FemaleKoiVariety { get; set; }


        public int? PondId { get; set; }
        public string? PondName { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public BreedingStatus Status { get; set; }
        public BreedingResult Result { get; set; }
        public string? Note { get; set; }

        public int? TotalEggs { get; set; }
        public double? FertilizationRate { get; set; }
        public double? CurrentSurvivalRate { get; set; }
        public int? TotalFishQualified { get; set; }
        public int? TotalPackage { get; set; }
        public ICollection<KoiFishResponseDTO>? KoiFishes { get; set; }

        public EggBatchResponseDTO? Batch { get; set; }

        public FryFishResponseDTO? FryFish { get; set; }

        public ClassificationStageResponseDTO? ClassificationStage { get; set; }
    }
}
