using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishRequestDTO
    {
        [Required(ErrorMessage = "PondId is required.")]
        public int PondId { get; set; }

        [Required(ErrorMessage = "VarietyId is required.")]
        public int VarietyId { get; set; }

        public int? BreedingProcessId { get; set; }

        [Required(ErrorMessage = "RFID is required.")]
        [MaxLength(50, ErrorMessage = "RFID cannot exceed 50 characters.")]
        public string RFID { get; set; }

        public FishSize? Size { get; set; }

        public KoiType Type { get; set; }
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "HealthStatus is required.")]
        public HealthStatus HealthStatus { get; set; }
        public string? Origin { get; set; }


        // Cho phép gửi nhiều ảnh hoặc video
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
