using System;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class WaterAlert
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public Pond Pond { get; set; }
        public string ParameterName { get; set; }
        public double MeasuredValue { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsResolved { get; set; }
        public int? ResolvedByUserId { get; set; }
        public ApplicationUser? ResolvedBy { get; set; }
    }
}
