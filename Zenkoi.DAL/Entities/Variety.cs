using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class Variety
    {
        public int Id { get; set; }

        public string VarietyName { get; set; }

        public string Characteristic { get; set; }

        public string OriginCountry { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<KoiFish> KoiFishes { get; set; }

        public ICollection<VarietyPattern> VarietyPatterns { get; set; } = new List<VarietyPattern>();
        public ICollection<VarietyPacketFish> VarietyPacketFishes { get; set; } = new List<VarietyPacketFish>();
    }
}