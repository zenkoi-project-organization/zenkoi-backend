using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.PondTypeDTOs
{
    public class PondTypeResponseDTO
    {
        public int PondTypeID { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public int? RecommendedCapacity { get; set; }
    }
}
