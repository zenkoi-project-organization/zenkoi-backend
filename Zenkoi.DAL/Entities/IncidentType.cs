using System.Collections.Generic;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class IncidentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public SeverityLevel SeverityLevel { get; set; }
        public bool? RequiresQuarantine { get; set; }
        public string? AffectsBreeding {  get; set; }
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
