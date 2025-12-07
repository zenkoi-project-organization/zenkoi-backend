using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class IncubationDailyRecord
    {
        public int Id { get; set; }
        public int EggBatchId { get; set; }
        public DateTime? DayNumber { get; set; } = DateTime.UtcNow;
        public int? HealthyEggs { get; set; }
        public int? RottenEggs { get; set; }
        public int? HatchedEggs { get; set; }
        public bool Success { get; set; } 
        public DateTime UpdatedAt { get; set;}
        // Navigation
        public EggBatch EggBatch { get; set; }
    }
}