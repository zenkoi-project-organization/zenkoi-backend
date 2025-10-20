using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities
{
    public class BreedingProcess
    {
        public int Id { get; set; }

        public int MaleKoiId { get; set; }
        public int FemaleKoiId { get; set; }
        public int? PondId { get; set; }
       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public BreedingStatus Status { get; set; }
        public string? Note { get; set; }
        public BreedingResult Result { get; set; }

        public int? TotalFishQualified { get; set; }
        public int? TotalPackage { get; set; }

        [ForeignKey(nameof(MaleKoiId))]
        public KoiFish MaleKoi { get; set; }

        [ForeignKey(nameof(FemaleKoiId))]
        public KoiFish FemaleKoi { get; set; }

        [ForeignKey(nameof(PondId))]
        public Pond? Pond { get; set; }
        public ICollection<KoiFish> KoiFishes { get; set; } = new List<KoiFish>();
        public EggBatch? Batch { get; set; }
        public FryFish? FryFish { get; set; }
        public ClassificationStage? ClassificationStage { get; set;}
    }
}
