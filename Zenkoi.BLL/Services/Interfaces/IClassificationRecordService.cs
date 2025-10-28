using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IClassificationRecordService 
    {
        Task<PaginatedList<ClassificationRecordResponseDTO>> GetAllAsync(ClassificationRecordFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<ClassificationRecordResponseDTO?> GetByIdAsync(int id);
        Task<ClassificationRecordResponseDTO> CreateAsync(ClassificationRecordRequestDTO dto);
        Task<bool> UpdateAsync(int id, ClassificationRecordUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<ClassificationRecordResponseDTO> CreateV2Async(ClassificationRecordV2RequestDTO dto);
        Task<ClassificationRecordResponseDTO> CreateV3Async(ClassificationRecordV3RequestDTO dto);
        Task<ClassificationSummaryDTO> GetSummaryAsync(int classificationStageId);
    }
}
