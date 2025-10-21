using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IAreaService
    {
        Task<PaginatedList<AreaResponseDTO>> GetAllAsync(AreaFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<AreaResponseDTO?> GetByIdAsync(int id);
        Task<AreaResponseDTO> CreateAsync(AreaRequestDTO dto);
        Task<bool> UpdateAsync(int id, AreaRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
