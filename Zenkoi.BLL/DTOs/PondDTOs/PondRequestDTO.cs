using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondDTOs
{
    public class PondRequestDTO
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
    }
}
