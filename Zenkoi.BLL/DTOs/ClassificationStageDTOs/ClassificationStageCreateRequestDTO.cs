using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.ClassificationStageDTOs
{
    public class ClassificationStageCreateRequestDTO
    {
        public int BreedingProcessId { get; set; }
        public int PondId { get; set; }
        public string? Notes { get; set; }
    }
}
