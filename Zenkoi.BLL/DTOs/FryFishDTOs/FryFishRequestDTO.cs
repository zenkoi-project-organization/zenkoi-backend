using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.FryFishDTOs
{
    public class FryFishRequestDTO
    {
        public int BreedingProcessId { get; set; }
        public int PondId { get; set; }
        public int? InitialCount { get; set; }
    }
}
