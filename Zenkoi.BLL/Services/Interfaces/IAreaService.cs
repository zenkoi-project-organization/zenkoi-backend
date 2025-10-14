using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IAreaService
    {
        Task<IEnumerable<AreaResponseDTO>> GetAllAsync();
        Task<AreaResponseDTO?> GetByIdAsync(int id);
        Task<AreaResponseDTO> CreateAsync(AreaRequestDTO dto);
        Task<bool> UpdateAsync(int id, AreaRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
