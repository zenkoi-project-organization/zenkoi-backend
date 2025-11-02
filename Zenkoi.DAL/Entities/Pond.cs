using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class Pond
    {
        public int Id { get; set; }

        public int PondTypeId { get; set; }

        public int AreaId { get; set; }

        public string PondName { get; set; }
        public PondStatus PondStatus { get; set; }
        public string Location { get; set; }

        public double? CapacityLiters { get; set; }
        public double? CurrentCapacity { get; set; }
        public double? DepthMeters { get; set; }
        public double? LengthMeters { get; set; }
        public double? WidthMeters { get; set; }
        public int? MaxFishCount { get; set; }
        public int? CurrentCount { get; set; }
        public DateTime CreatedAt { get; set; }

        public PondType PondType { get; set; }
        public Area Area { get; set; }
        public ICollection<BreedingProcess> BreedingProcesses { get; set; } = new List<BreedingProcess>();
        public ICollection<EggBatch> EggBatches { get; set; } = new List<EggBatch>();
        public ICollection<FryFish> FryFishes { get; set; } = new List<FryFish>();
        public ICollection<KoiFish> KoiFishes { get; set; }
        public ICollection<WaterParameterRecord> WaterParameters { get; set; } = new List<WaterParameterRecord>();
        public ICollection<PondAssignment> PondAssignments { get; set; } = new List<PondAssignment>();
        public ICollection<PondPacketFish> PondPacketFishes { get; set; } = new List<PondPacketFish>();
        public ICollection<PondIncident> PondIncidents { get; set; } = new List<PondIncident>();
    }
}