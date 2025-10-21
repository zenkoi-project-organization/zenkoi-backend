using System;
using System.Collections.Generic;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishResponseDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string? Size { get; set; }
        public KoiType Type { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public HealthStatus HealthStatus { get; set; }

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        public decimal? SellingPrice { get; set; }
        public string? BodyShape { get; set; }
        public string? Description { get; set; }
        public string? Origin { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public PondBasicDTO Pond { get; set; }
        public VarietyBasicDTO Variety { get; set; }
        public BreedingProcessBasicDTO? BreedingProcess { get; set; }
    }

    public class PondBasicDTO
    {
        public int Id { get; set; }
        public string PondName { get; set; }
    }

    public class VarietyBasicDTO
    {
        public int Id { get; set; }
        public string VarietyName { get; set; }
        public string Characteristic { get; set; }
        public string OriginCountry { get; set; }
    }

    public class BreedingProcessBasicDTO
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
    }
}
