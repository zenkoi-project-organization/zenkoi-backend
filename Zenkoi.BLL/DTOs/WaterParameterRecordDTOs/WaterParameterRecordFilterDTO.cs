using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.WaterParameterRecordDTOs
{
    public class WaterParameterRecordFilterDTO
    {
        public int? PondId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? RecordedByUserId { get; set; }
        public string? NotesContains { get; set; }
    }
}
