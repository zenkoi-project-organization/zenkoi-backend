using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class KoiFishFilterRequestDTO
    {
        public string? Search { get; set; }
        public Gender? Gender { get; set; }
        public HealthStatus? Health { get; set; }
        public int? VarietyId { get; set; }
        public FishSize? FishSize { get; set; }
        public int? PondId { get; set; }
        public string? Origin { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }


    }
}
