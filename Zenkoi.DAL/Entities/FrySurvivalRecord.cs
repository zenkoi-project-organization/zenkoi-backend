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

        public DateTime? DayNumber { get; set; } = DateTime.Now;
        public double? SurvivalRate { get; set; }
        public int? CountAlive { get; set; }

        public string? Note { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public bool Success { get; set; } = false;

        // Navigation
        public FryFish FryFish { get; set; }
    }
}