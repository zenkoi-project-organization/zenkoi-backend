using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs
{
    public class WaterParameterThresholdResponseDTO
    {
        public int Id { get; set; }
        public string ParameterName { get; set; } = default!;
        public string Unit { get; set; } = default!;
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public int? PondTypeId { get; set; }
        public string? PondTypeName { get; set; } 
    }
}