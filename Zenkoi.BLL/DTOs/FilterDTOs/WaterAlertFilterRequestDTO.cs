using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class WaterAlertFilterRequestDTO
    {
        public int? PondId { get; set; }
        public bool? IsResolved { get; set; }
        public bool? IsSeen { get; set; }
        public AlertType? AlertType { get; set; }
        public SeverityLevel? Severity { get; set; }
    }
}
