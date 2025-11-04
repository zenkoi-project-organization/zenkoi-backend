using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PondTypeDTOs
{
    public class PondTypeResponseDTO
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int? RecommendedQuantity { get; set; }
    }
}
