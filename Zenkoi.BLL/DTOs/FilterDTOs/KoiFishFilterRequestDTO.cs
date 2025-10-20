using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class KoiFishFilterRequestDTO
    {
        public string? Search { get; set; }
        public string? Gender { get; set; }
        public string? Health { get; set; }
        public int? VarietyId { get; set; }
        public int? PondId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public double? MinSize { get; set; }
        public double? MaxSize { get; set; }
    }
}
