using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IFrySurvivalRecordService
    {
        Task<PaginatedList<FrySurvivalRecordResponseDTO>> GetAllVarietiesAsync(int pageIndex = 1, int pageSize = 10);
        Task<FrySurvivalRecordResponseDTO?> GetByIdAsync(int id);
        Task<FrySurvivalRecordResponseDTO> CreateAsync(FrySurvivalRecordRequestDTO dto);
        Task<bool> UpdateAsync(int id, FrySurvivalRecordUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
