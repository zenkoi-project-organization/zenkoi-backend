using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class Pond
    {
        public int Id { get; set; }

        public int PondTypeId { get; set; }

        public int AreaId { get; set; }

        public string PondName { get; set; }
        public string Location { get; set; }
        public string PondStatus { get; set; }

        public double? CapacityLiters { get; set; }
        public double? DepthMeters { get; set; }
        public double? LengthMeters { get; set; }
        public double? WidthMeters { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public PondType PondType { get; set; }
        public Area Area { get; set; }
        public BreedingProcess BreedingProcess { get; set; }
        public EggBatch EggBatch { get; set; }
        public FryFish FryFish { get; set; }
        public ICollection<KoiFish> KoiFishes { get; set; }

       //public ICollection<PondPacketFish> PondPacketFishes { get; set; }
    }
}