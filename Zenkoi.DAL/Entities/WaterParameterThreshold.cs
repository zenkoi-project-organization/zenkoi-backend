using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class WaterParameterThreshold
    {
        public int Id { get; set; }
        public WaterParameterType ParameterName { get; set; }
        public string Unit {  get; set; } = "";
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int? PondTypeId { get; set; }
        public PondType? PondType { get; set; }
    }
}
