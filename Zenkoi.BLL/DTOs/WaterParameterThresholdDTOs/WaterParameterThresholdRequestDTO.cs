using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs
{
    public class WaterParameterThresholdRequestDTO
    {
        [Required]
        public WaterParameterType ParameterName { get; set; }

        public string Unit { get; set; } = default!;

        [Required]
        public double MinValue { get; set; }

        [Required]
        public double MaxValue { get; set; }

        public int? PondTypeId { get; set; }
    }
}