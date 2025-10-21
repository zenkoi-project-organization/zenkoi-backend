using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IEggBatchService
    {
        Task<PaginatedList<EggBatchResponseDTO>> GetAllEggBatchAsync(EggBatchFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<EggBatchResponseDTO?> GetByIdAsync(int id);
        Task<EggBatchResponseDTO> CreateAsync(EggBatchRequestDTO dto);
        Task<bool> UpdateAsync(int id, EggBatchUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
