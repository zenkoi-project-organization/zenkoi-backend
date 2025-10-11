using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class FryFish
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int PondId { get; set; }

        public int? InitialCount { get; set; }
        public string Status { get; set; }
        public double? CurrentSurvivalRate { get; set; }

        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public Pond Pond { get; set; }
        public ICollection<FrySurvivalRecord> FrySurvivalRecords { get; set; }
    }
}