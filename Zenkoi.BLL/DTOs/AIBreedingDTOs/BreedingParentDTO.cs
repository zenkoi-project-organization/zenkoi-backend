using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingParentDTO
    {
        public int Id { get; set; }
        public string Variety { get; set; } = "";
        public string Gender { get; set; } = "";
        public double Size { get; set; }
        public double Weight { get; set; }
        public string Health { get; set; } = "";
        public string? Note { get; set; }
        public List<BreedingRecordDTO> BreedingHistory { get; set; } = new();
    }
}
