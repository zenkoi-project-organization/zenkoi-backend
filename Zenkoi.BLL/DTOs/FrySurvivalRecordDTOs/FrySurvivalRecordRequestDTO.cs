using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs
{
    public class FrySurvivalRecordRequestDTO
    {
        public int FryFishId { get; set; }
        public int DayNumber { get; set; }
        public int? CountAlive { get; set; }
        public string? Note { get; set; }
        public bool Success { get; set; }
    }
}
