using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IIncubationDailyRecordService
    {
        Task<PaginatedList<IncubationDailyRecordResponseDTO>> GetAllByEggBatchIdAsync(int eggBatchId,int pageIndex = 1, int pageSize = 10);
        Task<IncubationDailyRecordResponseDTO?> GetByIdAsync(int id);
        Task<IncubationDailyRecordResponseDTO> CreateAsync(IncubationDailyRecordRequestDTO dto);
        Task<bool> UpdateAsync(int id, IncubationDailyRecordRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
