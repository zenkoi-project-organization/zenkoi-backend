using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IVarietyService
    {
        Task<PaginatedList<VarietyResponseDTO>> GetAllVarietiesAsync(VarietyFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<VarietyResponseDTO?> GetByIdAsync(int id);
        Task<VarietyResponseDTO> CreateAsync(VarietyRequestDTO dto);
        Task<bool> UpdateAsync(int id, VarietyRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
