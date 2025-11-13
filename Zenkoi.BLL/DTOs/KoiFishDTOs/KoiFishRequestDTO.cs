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
        public string RFID { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Size must be a positive number.")]
        public double? Size { get; set; }

        [Required(ErrorMessage = "Koi Type is required.")]
        public KoiType Type { get; set; }
        [Required(ErrorMessage = "Koi Pattern Type is required.")]
        public KoiPatternType PatternType { get; set; }

        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "HealthStatus is required.")]
        public HealthStatus HealthStatus { get; set; }

        [Required(ErrorMessage = "SaleStatus is required.")]
        public SaleStatus SaleStatus { get; set; } = SaleStatus.NotForSale;

        [MaxLength(100, ErrorMessage = "Origin cannot exceed 100 characters.")]
        public string? Origin { get; set; }

        public List<string>? Images { get; set; } = new();
        public List<string>? Videos { get; set; } = new();

        [Range(0, double.MaxValue, ErrorMessage = "SellingPrice must be a positive value.")]
        public decimal? SellingPrice { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public bool IsMutated { get; set; } = false;

        public string? MutationDescription { get; set; }
    }
}
