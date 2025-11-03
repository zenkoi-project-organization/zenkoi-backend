using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class ShippingDistance
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinDistanceKm { get; set; }       
        public int MaxDistanceKm { get; set; }
        public decimal PricePerKm { get; set; }
        public decimal BaseFee { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }


    }
}
