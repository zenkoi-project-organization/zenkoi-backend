using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IVarietyService
    {
        Task<IEnumerable<VarietyResponseDTO>> GetAllAsync();
        Task<VarietyResponseDTO?> GetByIdAsync(int id);
        Task<VarietyResponseDTO> CreateAsync(VarietyRequestDTO dto);
        Task<bool> UpdateAsync(int id, VarietyRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
