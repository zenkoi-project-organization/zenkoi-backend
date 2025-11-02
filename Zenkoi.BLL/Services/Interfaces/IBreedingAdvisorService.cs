using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs.AIPairAnalysisDTOs;
using Zenkoi.BLL.DTOs.BreedingDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IBreedingAdvisorService
    {
        Task<AIBreedingResponseDTO> RecommendPairsAsync(BreedingRequestDTO request);
        Task<AIPairAnalysisResponseDTO> AnalyzePairAsync(AIPairAnalysisRequestDTO request);
    }
}

