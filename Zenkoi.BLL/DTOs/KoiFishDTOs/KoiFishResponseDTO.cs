using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishResponseDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public double? Size { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public string ImagesVideos { get; set; }
        public decimal? SellingPrice { get; set; }
        public string BodyShape { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Liên kết cơ bản
        public PondBasicDTO Pond { get; set; }
        public VarietyBasicDTO Variety { get; set; }
        public BreedingProcessBasicDTO BreedingProcess { get; set; }
    }

    // Chỉ trả thông tin cần thiết để tránh vòng lặp dữ liệu
    public class PondBasicDTO
    {
        public int Id { get; set; }
        public string PondName { get; set; }
    }

    public class VarietyBasicDTO
    {
        public int Id { get; set; }
        public string VarietyName { get; set; }
    }

    public class BreedingProcessBasicDTO
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
    }
}