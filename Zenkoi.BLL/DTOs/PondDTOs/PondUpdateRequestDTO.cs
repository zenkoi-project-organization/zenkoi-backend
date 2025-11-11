using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondDTOs
{
    public class PondUpdateRequestDTO
    {
        [Required]
        public int PondTypeId { get; set; }

        [Required]
        public int AreaId { get; set; }

        [Required]
        [StringLength(100)]
        public string PondName { get; set; }
        public string? Location { get; set; }
        public PondStatus PondStatus { get; set; } = PondStatus.Empty;
        public double? CurrentCapacity { get; set; }
        public double? DepthMeters { get; set; }
        public double? LengthMeters { get; set; }
        public double? WidthMeters { get; set; }
        public WaterRecordDTO? record { get; set; }

    }
}
