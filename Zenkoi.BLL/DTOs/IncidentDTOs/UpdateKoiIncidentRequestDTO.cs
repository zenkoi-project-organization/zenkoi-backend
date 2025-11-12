using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class UpdateKoiIncidentRequestDTO
    {
        public HealthStatus? AffectedStatus { get; set; }
        [MaxLength(1000)]
        public string? SpecificSymptoms { get; set; }
        public bool? RequiresTreatment { get; set; }
        public bool? IsIsolated { get; set; }
        [MaxLength(2000)]
        public string? TreatmentNotes { get; set; }
        public DateTime? RecoveredAt { get; set; }
    }
}
