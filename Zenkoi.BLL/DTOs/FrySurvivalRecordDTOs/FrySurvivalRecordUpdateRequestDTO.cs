using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs
{
    public class FrySurvivalRecordUpdateRequestDTO
    {
        public int? CountAlive { get; set; }
        public string? Note { get; set; }
        public bool Success { get; set; }
    }
}
