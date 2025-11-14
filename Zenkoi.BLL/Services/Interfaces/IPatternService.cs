using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PatternDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPatternService
    {
        Task<PaginatedList<PatternResponseDTO>> GetAllAsync( int pageIndex = 1, int pageSize = 10);
        Task<PatternResponseDTO?> GetByIdAsync(int id);
        Task<PatternResponseDTO> CreateAsync(PatternRequestDTO dto);
        Task<bool> UpdateAsync(int id, PatternRequestDTO dto);
        Task<bool> DeleteAsync(int id);

        Task<bool> AssignPatternToVarietyAsync(int varietyId, int patternId);
        Task<bool> RemovePatternFromVarietyAsync(int varietyId, int patternId);

        Task<PaginatedList<PatternResponseDTO>> GetPatternsByVarietyAsync(
              int varietyId,
              int pageIndex = 1,
              int pageSize = 10);
        Task<PaginatedList<VarietyResponseDTO>> GetVarietiesByPatternAsync(
       int patternId,
       int pageIndex = 1,
       int pageSize = 10);
    }
}
