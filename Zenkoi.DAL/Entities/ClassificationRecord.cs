using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.DAL.Entities
{
    public class ClassificationRecord
    {
        public int Id { get; set; }
        public int ClassificationStageId { get; set; }

        public int StageNumber { get; set; } = 0;
        public int? HighQualifiedCount { get; set; } = 0;
        public int? ShowQualifiedCount { get; set; } = 0; 
        public int? PondQualifiedCount { get; set; } = 0;
        public int? CullQualifiedCount { get; set; } = 0;

        public string? Notes { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ClassificationStage ClassificationStage { get; set; }
    }
}