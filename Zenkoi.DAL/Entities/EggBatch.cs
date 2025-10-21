using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class EggBatch
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int? PondId { get; set; }
        public int? Quantity { get; set; }
        public int? TotalHatchedEggs { get; set; } = 0;
        public double? FertilizationRate { get; set; }

        public EggBatchStatus Status { get; set; }
        public DateTime? HatchingTime { get; set; }
        public DateTime? SpawnDate { get; set; }
        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public Pond? Pond { get; set; }
        public ICollection<IncubationDailyRecord> IncubationDailyRecords { get; set; }
    }
}