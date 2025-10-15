using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class KoiFish
    {
        public int Id { get; set; }

        public int PondId { get; set; }
        public int? BreedingProcessId { get; set; }
        public int VarietyId { get; set; }

        public string RFID { get; set; }
        public double? Size { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public string ImagesVideos { get; set; }
        public decimal? SellingPrice { get; set; }
        public string BodyShape { get; set; }
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public Pond Pond { get; set; }
        public Variety Variety { get; set; }
        public ICollection<KoiIncident> KoiIncidents { get; set; } = new List<KoiIncident>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        
    }
}