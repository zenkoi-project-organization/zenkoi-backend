using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.AIBreedingDTOs
{
    public class AIBreedingResponseDTO
    { 
        public List<BreedingPairResult> RecommendedPairs { get; set; } = new();
    }
}
