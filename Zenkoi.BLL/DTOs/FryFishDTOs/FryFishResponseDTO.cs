using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FryFishDTOs
{
    public class FryFishResponseDTO
    {
        public int Id { get; set; }
        public int BreedingProcessId { get; set; }
        public int? InitialCount { get; set; }
        public FryFishStatus Status { get; set; }
        public double? CurrentSurvivalRate { get; set; }
       public ICollection<FrySurvivalRecordResponseDTO>? FrySurvivalRecords { get; set; }
    }
}
