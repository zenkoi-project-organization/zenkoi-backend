using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPondTypeService
    {
        Task<PaginatedList<PondTypeResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
        Task<PondTypeResponseDTO?> GetByIdAsync(int id);
        Task<PondTypeResponseDTO> CreateAsync(PondTypeRequestDTO dto);
        Task<bool> UpdateAsync(int id, PondTypeRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
