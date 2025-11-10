using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishUpdateRequestDTO
    {
        public int? PondId { get; set; }
        public int? VarietyId { get; set; }

        [MaxLength(50, ErrorMessage = "RFID cannot exceed 50 characters.")]
        public string? RFID { get; set; }

        public FishSize? Size { get; set; }
        public KoiType? Type { get; set; }
        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }
        public HealthStatus? HealthStatus { get; set; }
        public string? PatternType { get; set; }
        public SaleStatus? SaleStatus { get; set; }

        [MaxLength(200, ErrorMessage = "Origin cannot exceed 200 characters.")]
        public string? Origin { get; set; }

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellingPrice must be a positive value.")]
        public decimal? SellingPrice { get; set; }

        [MaxLength(100, ErrorMessage = "BodyShape cannot exceed 100 characters.")]
        public string? BodyShape { get; set; }

        [MaxLength(100, ErrorMessage = "ColorPattern cannot exceed 100 characters.")]
        public string? ColorPattern { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }
    }
}
