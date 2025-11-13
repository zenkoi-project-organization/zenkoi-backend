using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class Pattern
    {
        public int Id { get; set; }
        public  string PatternName { get; set; }
        public string Description { get; set; }
        public ICollection<VarietyPattern> VarietyPatterns { get; set; } = new List<VarietyPattern>();

    }
}
