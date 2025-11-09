using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class WaterAlert
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public WaterParameterType ParameterName { get; set; }
        public double MeasuredValue { get; set; }
        public AlertType AlertType { get; set; }
        public SeverityLevel Severity { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolveAt { get; set; }
        public bool IsResolved { get; set; } 
        public int? ResolvedByUserId { get; set; }
        public Pond Pond { get; set; }
        public WaterParameterRecord? WaterParameterRecord { get; set; }
        public ApplicationUser? ResolvedBy { get; set; }
    }
}
