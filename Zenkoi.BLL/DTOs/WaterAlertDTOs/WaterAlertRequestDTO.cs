using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WaterAlertDTOs
{
    public class WaterAlertRequestDTO
    {
        public int PondId { get; set; }
        public WaterParameterType ParameterName { get; set; }
        public double MeasuredValue { get; set; }
        public AlertType AlertType { get; set; }
        public SeverityLevel Severity { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsResolved { get; set; }
    }
}
