using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.KoiFishDTOs
{
    public class KoiFishResponseDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; } = string.Empty;

        public string? Size { get; set; }

        public KoiType Type { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public string? Pattern { get; set; }
        public SaleStatus SaleStatus { get; set; }

        public List<string>? Images { get; set; } = new();
        public List<string>? Videos { get; set; } = new();

        public decimal? SellingPrice { get; set; }
        public string? Description { get; set; }
        public string? Origin { get; set; }

        public bool IsMutated { get; set; }

        public string? MutationDescription { get; set; }
        public double? MutationRate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public PondBasicDTO Pond { get; set; } = new();
        public VarietyBasicDTO Variety { get; set; } = new();
        public BreedingProcessBasicDTO? BreedingProcess { get; set; }
    }

    public class PondBasicDTO
    {
        public int Id { get; set; }
        public string PondName { get; set; } = string.Empty;
    }

    public class VarietyBasicDTO
    {
        public int Id { get; set; }
        public string VarietyName { get; set; } = string.Empty;
        public string? Characteristic { get; set; }
        public string? OriginCountry { get; set; }
    }

    public class BreedingProcessBasicDTO
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
