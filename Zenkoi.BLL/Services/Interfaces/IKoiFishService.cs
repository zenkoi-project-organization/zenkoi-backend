using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IKoiFishService
    {
        Task<PaginatedList<KoiFishResponseDTO>> GetAllKoiFishAsync(int pageIndex = 1, int pageSize = 10);
        Task<KoiFishResponseDTO?> GetByIdAsync(int id);
        Task<KoiFishResponseDTO> CreateAsync(KoiFishRequestDTO dto);
        Task<bool> UpdateAsync(int id, KoiFishRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
