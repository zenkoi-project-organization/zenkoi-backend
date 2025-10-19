using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.BreedingDTOs
{
    public class BreedingProcessRequestDTO
    {
        public int MaleKoiId { get; set; }
        public int FemaleKoiId { get; set; }
        public int PondId { get; set; }
    }
}
