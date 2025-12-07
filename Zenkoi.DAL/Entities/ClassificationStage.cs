using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class ClassificationStage
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int TotalCount { get; set; }
        public ClassificationStatus Status { get; set; }
        public int? HighQualifiedCount { get; set; }
        public int? ShowQualifiedCount { get; set; }
        public int? PondQualifiedCount { get; set; }
        public int? CullQualifiedCount { get; set; } 
        public string Notes { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } 

        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public ICollection<ClassificationRecord> ClassificationRecords { get; set; }
    }
}