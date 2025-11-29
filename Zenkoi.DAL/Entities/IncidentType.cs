using System.Collections.Generic;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class IncidentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public SeverityLevel DefaultSeverity { get; set; }
        public bool? AffectsBreeding {  get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
