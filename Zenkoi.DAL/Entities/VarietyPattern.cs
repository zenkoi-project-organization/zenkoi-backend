using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class VarietyPattern
    {
        public int Id { get; set; }
        public int PatternId { get; set; }
        public int VarietyId { get; set; }

        public Pattern Pattern { get; set; }
        public Variety Variety { get; set; }
    }
}
