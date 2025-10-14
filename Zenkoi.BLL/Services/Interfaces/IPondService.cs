using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPondService
    {
        Task<IEnumerable<PondResponseDTO>> GetAllAsync();
        Task<PondResponseDTO?> GetByIdAsync(int id);
        Task<PondResponseDTO> CreateAsync(PondRequestDTO dto);
        Task<bool> UpdateAsync(int id, PondRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
