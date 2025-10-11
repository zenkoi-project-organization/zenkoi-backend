using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class ClassificationStage
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }

        public int? TotalCount { get; set; }
        public int? HighQualifiedCount { get; set; }
        public int? QualifiedCount { get; set; }
        public int? UnqualifiedCount { get; set; }
        public string Notes { get; set; }

        // Navigation
        public BreedingProcess BreedingProcess { get; set; }
        public ICollection<ClassificationRecord> ClassificationRecords { get; set; }
    }
}