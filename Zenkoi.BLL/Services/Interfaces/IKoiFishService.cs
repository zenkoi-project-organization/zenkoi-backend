using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFishDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IKoiFishService
    {
        Task<IEnumerable<KoiFishResponseDTO>> GetAllAsync();
        Task<KoiFishResponseDTO?> GetByIdAsync(int id);
        Task<KoiFishResponseDTO> CreateAsync(KoiFishRequestDTO dto);
        Task<bool> UpdateAsync(int id, KoiFishRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
