using System;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class WaterParameterRecord
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public Pond Pond { get; set; }
        public double? PHLevel { get; set; }
        public double? TemperatureCelsius { get; set; }
        public double? OxygenLevel { get; set; }
        public double? AmmoniaLevel { get; set; }
        public double? NitriteLevel { get; set; }
        public double? NitrateLevel { get; set; }
        public double? CarbonHardness { get; set; }
        public double? WaterLevelMeters { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public int? RecordedByUserId { get; set; }
        public ApplicationUser? RecordedBy { get; set; }
        public string? Notes { get; set; }
    }
}
