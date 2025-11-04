using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs
{
    public class WaterParameterThresholdRequestDTO
    {
        [Required]
        public string ParameterName { get; set; } = default!;

        [Required]
        public string Unit { get; set; } = default!;

        [Required]
        public double MinValue { get; set; }

        [Required]
        public double MaxValue { get; set; }

        public string? Notes { get; set; }

        public int? PondTypeId { get; set; }
    }
}