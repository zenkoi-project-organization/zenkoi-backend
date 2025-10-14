using System;
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

        [Range(0.1, double.MaxValue, ErrorMessage = "Size must be greater than 0.")]
        public double? Size { get; set; }

        public DateTime? BirthDate { get; set; }

        [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public Gender Gender { get; set; }

        [MaxLength(100, ErrorMessage = "HealthStatus cannot exceed 100 characters.")]
        public HealthStatus HealthStatus { get; set; }

        [MaxLength(500, ErrorMessage = "ImagesVideos cannot exceed 500 characters.")]
        public string ImagesVideos { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "SellingPrice must be a positive value.")]
        public decimal? SellingPrice { get; set; }

        [MaxLength(100, ErrorMessage = "BodyShape cannot exceed 100 characters.")]
        public string BodyShape { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }
    }
}
