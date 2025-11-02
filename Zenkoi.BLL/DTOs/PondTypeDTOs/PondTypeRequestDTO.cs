using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondTypeDTOs
{
    public class PondTypeRequestDTO
    {
        public string TypeName { get; set; }
        public string Description { get; set; }
        public TypeOfPond Type { get; set; }
        public int? RecommendedCapacity { get; set; }

    }
}
