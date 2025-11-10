using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class KoiIncident
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }

        public int KoiFishId { get; set; }
        public KoiFish KoiFish { get; set; }
        public KoiAffectedStatus AffectedStatus { get; set; } = KoiAffectedStatus.Exposed;
        public string? SpecificSymptoms { get; set; }
        public bool RequiresTreatment { get; set; }
        public bool IsIsolated { get; set; }
        public DateTime AffectedFrom { get; set; } = DateTime.UtcNow;
        public DateTime? RecoveredAt { get; set; }
        public string? TreatmentNotes { get; set; }
    }
}
