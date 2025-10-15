using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

namespace Zenkoi.BLL.DTOs.VarietyDTOs
{
    public class VarietyResponseDTO
    {
        public int Id { get; set; }
        public string VarietyName { get; set; }
        public string Characteristic { get; set; }
        public string OriginCountry { get; set; }

        
       // public ICollection<KoiFishBasicDTO> KoiFishes { get; set; }
    }
}