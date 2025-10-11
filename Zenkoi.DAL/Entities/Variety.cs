using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class Variety
    {
        public int Id { get; set; }

        public string VarietyName { get; set; }

        public string Characteristic { get; set; }
   
        public string OriginCountry { get; set; }

        public ICollection<KoiFish> KoiFishes { get; set; }
    }
}