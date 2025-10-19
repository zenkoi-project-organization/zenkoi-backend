using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPondService
    {
        Task<PaginatedList<PondResponseDTO>> GetAllPondsAsync(int pageIndex = 1, int pageSize = 10);
        Task<PondResponseDTO?> GetByIdAsync(int id);
        Task<PondResponseDTO> CreateAsync(PondRequestDTO dto);
        Task<bool> UpdateAsync(int id, PondRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
