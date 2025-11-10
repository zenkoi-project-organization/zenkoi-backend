using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.PondDTOs
{
    public class WaterRecordDTO
    {
        public double? PHLevel { get; set; }

        public double? TemperatureCelsius { get; set; }

        public double? OxygenLevel { get; set; }

        public double? AmmoniaLevel { get; set; }

        public double? NitriteLevel { get; set; }

        public double? NitrateLevel { get; set; }

        public double? CarbonHardness { get; set; }

        public double? WaterLevelMeters { get; set; }
        public string? Notes { get; set; }
    }
}
