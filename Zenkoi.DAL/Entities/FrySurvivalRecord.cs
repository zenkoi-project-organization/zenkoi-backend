using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class FrySurvivalRecord
    {
        public int Id { get; set; }
        public int FryFishId { get; set; }

        public int DayNumber { get; set; }
        public double? SurvivalRate { get; set; }
        public int? CountAlive { get; set; }

        // Navigation
        public FryFish FryFish { get; set; }
    }
}