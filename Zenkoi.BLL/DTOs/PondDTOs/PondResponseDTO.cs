using System;
using System.Collections.Generic;
using Zenkoi.DAL.Enums;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.AreaDTOs;

namespace Zenkoi.BLL.DTOs.PondDTOs
{
    public class PondResponseDTO
    {
        public int Id { get; set; }

        public string PondName { get; set; }
        public string? Location { get; set; }
        public PondStatus PondStatus { get; set; }
        public int? MaxFishCount { get; set; }
        public int? CurrentCount { get; set; }
        public double? CurrentCapacity { get; set; }
        public double? CapacityLiters { get; set; }
        public double? DepthMeters { get; set; }
        public double? LengthMeters { get; set; }
        public double? WidthMeters { get; set; }
        public bool IsDeleted { get; set; } 

        public DateTime CreatedAt { get; set; }

        // Info liên quan
        public int PondTypeId { get; set; }
        public string PondTypeName { get; set; }

        public int AreaId { get; set; }
        public string AreaName { get; set; }

        public WaterRecordDTO record {  get; set; }

      
    }
}
