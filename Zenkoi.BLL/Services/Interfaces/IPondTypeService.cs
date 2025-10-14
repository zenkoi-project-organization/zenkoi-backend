using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPondTypeService
    {
        Task<IEnumerable<PondTypeResponseDTO>> GetAllAsync();
        Task<PondTypeResponseDTO?> GetByIdAsync(int id);
        Task<PondTypeResponseDTO> CreateAsync(PondTypeRequestDTO dto);
        Task<bool> UpdateAsync(int id, PondTypeRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
