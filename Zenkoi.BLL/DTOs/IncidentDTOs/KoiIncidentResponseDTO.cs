using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class KoiIncidentResponseDTO
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public int KoiFishId { get; set; }
        public string? KoiFishRFID { get; set; }
        public HealthStatus AffectedStatus { get; set; }
        public string? SpecificSymptoms { get; set; }
        public bool RequiresTreatment { get; set; }
        public bool IsIsolated { get; set; }
        public DateTime AffectedFrom { get; set; }
        public DateTime? RecoveredAt { get; set; }
        public string? TreatmentNotes { get; set; }
    }
}
