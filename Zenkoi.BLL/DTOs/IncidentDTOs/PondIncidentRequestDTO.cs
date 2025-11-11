using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class PondIncidentRequestDTO
    {
        [Required]
        public int PondId { get; set; }
        [MaxLength(2000)]
        public string? EnvironmentalChanges { get; set; }
        public bool RequiresWaterChange { get; set; }
        public int? FishDiedCount { get; set; }
        [MaxLength(2000)]
        public string? CorrectiveActions { get; set; }
        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}
