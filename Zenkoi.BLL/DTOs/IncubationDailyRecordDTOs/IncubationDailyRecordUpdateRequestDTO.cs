using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs
{
    public class IncubationDailyRecordUpdateRequestDTO
    {
        public int? HealthyEggs { get; set; }
        public int? HatchedEggs { get; set; }
        public bool Success { get; set; }
    }
}
