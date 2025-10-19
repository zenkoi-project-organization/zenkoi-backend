using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs
{
    public class FrySurvivalRecordResponseDTO
    {
        public int Id { get; set; }
        public int FryFishId { get; set; }
        public int DayNumber { get; set; }
        public double? SurvivalRate { get; set; }
        public int? CountAlive { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }      
        public int? InitialCount { get; set; }
    }
}
