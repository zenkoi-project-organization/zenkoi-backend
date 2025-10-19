using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class FryFish
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int? PondId { get; set; }

        public int? InitialCount { get; set; }       // số lượng ban đầu 
        public FryFishStatus Status { get; set; }
        public double? CurrentSurvivalRate { get; set; }

        public DateTime StartDate { get; set; }      
        public DateTime? EndDate { get; set; }
        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public Pond? Pond { get; set; }
        public ICollection<FrySurvivalRecord> FrySurvivalRecords { get; set; }
    }
}