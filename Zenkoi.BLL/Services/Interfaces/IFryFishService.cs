using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IFryFishService
    {
        Task<PaginatedList<FryFishResponseDTO>> GetAllAsync(FryFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<FryFishResponseDTO?> GetByIdAsync(int id);
        Task<FryFishResponseDTO> CreateAsync(FryFishRequestDTO dto);
        Task<bool> UpdateAsync(int id, FryFishUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
