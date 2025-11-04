using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IWaterParameterThresholdService
    {
        Task<PaginatedList<WaterParameterThresholdResponseDTO>> GetAllAsync(
            WaterParameterThresholdFilterDTO? filter = null,
            int pageIndex = 1,
            int pageSize = 10);

        Task<WaterParameterThresholdResponseDTO?> GetByIdAsync(int id);

        Task<WaterParameterThresholdResponseDTO> CreateAsync(WaterParameterThresholdRequestDTO dto);

        Task<WaterParameterThresholdResponseDTO?> UpdateAsync(int id, WaterParameterThresholdRequestDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
