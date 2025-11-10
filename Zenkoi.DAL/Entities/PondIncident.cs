using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class PondIncident
    {
        public int Id { get; set; }
        public int IncidentId { get; set; }
        public Incident Incident { get; set; }
        public int PondId { get; set; }
        public Pond Pond { get; set; }
        public string? EnvironmentalChanges { get; set; }
        public bool RequiresWaterChange { get; set; }
        public int? FishDiedCount { get; set; }
        public string? CorrectiveActions { get; set; }
        public string? Notes { get; set; }
    }
}
