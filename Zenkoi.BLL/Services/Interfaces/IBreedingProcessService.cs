using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IBreedingProcessService
    {
        Task<BreedingProcessResponseDTO> AddBreeding(BreedingProcessRequestDTO dto);
        Task<BreedingProcessResponseDTO> GetBreedingById(int id);
        Task<BreedingResponseDTO> GetDetailBreedingById(int id);
        Task<bool> UpdateSpawnedStatus(int id);
        Task<bool> CancelBreeding(int id);
        Task<double> GetOffspringInbreedingAsync(int maleId, int femaleId );
        Task<double> GetIndividualInbreedingAsync(int koiId );
        Task<PaginatedList<BreedingProcessResponseDTO>> GetAllBreedingProcess(BreedingProcessFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<KoiFishParentResponseDTO> GetKoiFishParentStatsAsync(int koiFishId);
        Task<List<BreedingParentDTO>> GetParentsWithPerformanceAsync(string? variety = null);
        Task<List<KoiFishResponseDTO>> GetAllKoiFishByBreedingProcessAsync(int breedingProcessId);
    }
}
