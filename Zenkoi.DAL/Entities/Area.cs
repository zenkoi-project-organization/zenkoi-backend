using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class Area
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public double? TotalAreaSQM { get; set; }
        public string Description { get; set; }

        public ICollection<Pond> Ponds { get; set; }
    }
}