using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class Incident
    {
        public int Id { get; set; }
        public int IncidentTypeId { get; set; }
        public IncidentType IncidentType { get; set; }
        public string IncidentTitle { get; set; }
        public string Description { get; set; }
        public IncidentStatus Status { get; set; } = IncidentStatus.Reported;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int ReportedByUserId { get; set; }
        public ApplicationUser ReportedBy { get; set; }
        public int? ResolvedByUserId { get; set; }
        public ApplicationUser? ResolvedBy { get; set; }
        public string? ResolutionNotes { get; set; }
        public ICollection<KoiIncident> KoiIncidents { get; set; } = new List<KoiIncident>();
        public ICollection<PondIncident> PondIncidents { get; set; } = new List<PondIncident>();
    }
}
