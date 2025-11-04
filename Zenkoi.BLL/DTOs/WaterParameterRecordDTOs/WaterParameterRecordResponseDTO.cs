using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterParameterRecordDTOs
{
    public class WaterParameterRecordResponseDTO
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public string PondName { get; set; } 

        public double? PHLevel { get; set; }
        public double? TemperatureCelsius { get; set; }
        public double? OxygenLevel { get; set; }
        public double? AmmoniaLevel { get; set; }
        public double? NitriteLevel { get; set; }
        public double? NitrateLevel { get; set; }
        public double? Turbidity { get; set; }
        public double? TotalChlorines { get; set; }
        public double? CarbonHardness { get; set; }
        public double? WaterLevelMeters { get; set; }

        public DateTime RecordedAt { get; set; }
        public int? RecordedByUserId { get; set; }
        public string? RecordedByUserName { get; set; } // Nếu có thông tin user
        public string? Notes { get; set; }
    }
}