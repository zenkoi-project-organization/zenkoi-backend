using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterParameterRecordDTOs
{
    public class WaterParameterRecordRequestDTO
    {
        [Required]
        public int PondId { get; set; }

        public double? PHLevel { get; set; }

        public double? TemperatureCelsius { get; set; }

        public double? OxygenLevel { get; set; }

        public double? AmmoniaLevel { get; set; }

        public double? NitriteLevel { get; set; }

        public double? NitrateLevel { get; set; }

        public double? CarbonHardness { get; set; }

        public double? WaterLevelMeters { get; set; }

        public DateTime? RecordedAt { get; set; }


        public string? Notes { get; set; }
    }
}
