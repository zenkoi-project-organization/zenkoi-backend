using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class BreedingParentDTO
    {
        public int Id { get; set; }
        public string RFID { get; set; }
        public string Variety { get; set; } = "";
        public string Gender { get; set; } = "";
        public string image { get; set; }
        public string Size { get; set; }
        public double Age { get; set; }
        public string Health { get; set; } = "";
        public bool IsMutated { get; set; }
        public string? MutationDescription { get; set; }
        public string? Note { get; set; }
        public List<BreedingRecordDTO> BreedingHistory { get; set; } = new();
    }
}
