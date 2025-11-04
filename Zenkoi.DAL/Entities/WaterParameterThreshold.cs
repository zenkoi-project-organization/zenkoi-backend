using System;

namespace Zenkoi.DAL.Entities
{
    public class WaterParameterThreshold
    {
        public int Id { get; set; }
        public string ParameterName { get; set; } = default!;
        public string Unit {  get; set; } = "";
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int? PondTypeId { get; set; }
        public PondType? PondType { get; set; }
    }
}
