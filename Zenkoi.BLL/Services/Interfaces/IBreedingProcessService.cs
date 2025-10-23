using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IBreedingProcessService
    {
        public  Task<BreedingProcessResponseDTO> AddBreeding(BreedingProcessRequestDTO dto);
        public Task<BreedingProcessResponseDTO> GetBreedingById(int id);
        public Task<BreedingResponseDTO> GetDetailBreedingById(int id);
        public Task<bool> UpdateStatus(int id);
        public Task<double> GetOffspringInbreedingAsync(int maleId, int femaleId );
        Task<double> GetIndividualInbreedingAsync(int koiId );
        public Task<PaginatedList<BreedingProcessResponseDTO>> GetAllBreedingProcess(BreedingProcessFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10); 
    }
}
