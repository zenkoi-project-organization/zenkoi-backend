using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.BreedingDTOs
{
    public class BreedingProcessResponseDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;

        public int MaleKoiId { get; set; }
        public string? MaleKoiRFID { get; set; }
        public string? MaleKoiVariety { get; set; }

        public int FemaleKoiId { get; set; }
        
        public string? FemaleKoiRFID { get; set; }
        public string? FemaleKoiVariety { get; set; }

        public int? PondId { get; set; }
        public string? PondName { get; set; }
        public DateTime? HatchedTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalEggs { get; set; }
        public double? FertilizationRate { get; set; }
        public double? SurvivalRate { get; set; }

        public BreedingStatus Status { get; set; }
        public BreedingResult Result { get; set; }

        public string? Note { get; set; }

        public int? TotalFishQualified { get; set; }
        public int? TotalPackage { get; set; }

        public ICollection<KoiFishResponseDTO>? KoiFishes { get; set; }
    }
}