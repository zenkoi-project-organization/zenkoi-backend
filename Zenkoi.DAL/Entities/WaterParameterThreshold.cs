using System;

namespace Zenkoi.DAL.Entities
{
    public class WaterParameterThreshold
    {
        public int Id { get; set; }
        public string ParameterName { get; set; } = default!;
        public string Unit {  get; set; } = default!;
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string? Notes { get; set; }

        // apply to a pond type or global
        public int? PondTypeId { get; set; }
        public PondType? PondType { get; set; }
    }
}
