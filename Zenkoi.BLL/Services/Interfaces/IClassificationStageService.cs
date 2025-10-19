using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IClassificationStageService
    {
        Task<PaginatedList<ClassificationStageResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10);
        Task<ClassificationStageResponseDTO?> GetByIdAsync(int id);
        Task<ClassificationStageResponseDTO> CreateAsync(ClassificationStageCreateRequestDTO dto);
        Task<bool> UpdateAsync(int id, ClassificationStageUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
