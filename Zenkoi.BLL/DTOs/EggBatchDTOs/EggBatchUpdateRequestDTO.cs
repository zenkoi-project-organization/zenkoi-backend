using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.EggBatchDTOs
{
    public class EggBatchUpdateRequestDTO
    {
        public int PondId { get; set; }
        public int? Quantity { get; set; }
    }
}
