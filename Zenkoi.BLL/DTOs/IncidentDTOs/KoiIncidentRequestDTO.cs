using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class KoiIncidentRequestDTO
    {
        [Required]
        public int KoiFishId { get; set; }
        public KoiAffectedStatus AffectedStatus { get; set; } = KoiAffectedStatus.Exposed;
        [MaxLength(1000)]
        public string? SpecificSymptoms { get; set; }
        public bool RequiresTreatment { get; set; }
        public bool IsIsolated { get; set; }
        public DateTime AffectedFrom { get; set; } = DateTime.UtcNow;
        [MaxLength(2000)]
        public string? TreatmentNotes { get; set; }
    }
}
