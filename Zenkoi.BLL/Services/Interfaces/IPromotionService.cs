using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PromotionDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<PaginatedList<PromotionResponseDTO>> GetAllAsync(PromotionFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<PromotionResponseDTO?> GetByIdAsync(int id);
        Task<PromotionResponseDTO> CreateAsync(PromotionRequestDTO dto);
        Task<bool> UpdateAsync(int id, PromotionRequestDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<PromotionResponseDTO?> GetCurrentActivePromotionAsync();
    }
}
