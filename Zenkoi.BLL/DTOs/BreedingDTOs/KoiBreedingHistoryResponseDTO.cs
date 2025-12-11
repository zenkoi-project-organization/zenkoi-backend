using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.BreedingDTOs
{
    public class KoiBreedingHistoryResponseDTO
    {
        public int KoiId { get; set; }
        public string RFID { get; set; }
        public string VarietyName { get; set; }
        public string Gender { get; set; }
        public List<string>? Images { get; set; }
        public List<KoiBreedingRecordDTO> BreedingHistory { get; set; } = new();
    }
    public class KoiBreedingRecordDTO
    {
        public int BreedingProcessId { get; set; }
        public string Code { get; set; }
        public ParentKoiInfoDTO Partner { get; set; }
        public int? TotalEggs { get; set; }
        public double? FertilizationRate { get; set; }
        public double? HatchingRate { get; set; }
        public double? SurvivalRate { get; set; }
        public int? TotalFishQualified { get; set; }
        public double? MutationRate { get; set; }
        public int? TotalPackage { get; set; }

        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class ParentKoiInfoDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string VarietyName { get; set; }
        public bool IsMutated { get; set; }
        public string? MutationDescription { get; set; }
        public List<string>? Images { get; set; }
    }
}