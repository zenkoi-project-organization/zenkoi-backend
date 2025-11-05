using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IWaterParameterRecordService
    {
        Task<PaginatedList<WaterParameterRecordResponseDTO>> GetAllAsync(
            WaterParameterRecordFilterDTO? filter, int pageIndex = 1, int pageSize = 10);

        Task<WaterParameterRecordResponseDTO?> GetByIdAsync(int id);
        Task<WaterParameterRecordResponseDTO> CreateAsync(int userId,WaterParameterRecordRequestDTO dto);
        Task<WaterParameterRecordResponseDTO?> UpdateAsync(int id, WaterParameterRecordRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}