using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs
{
    public class WaterParameterThresholdFilterDTO
    {
        public WaterParameterType? ParameterName { get; set; }
        public int? PondTypeId { get; set; }
    }
}
