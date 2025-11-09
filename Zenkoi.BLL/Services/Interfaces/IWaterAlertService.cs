using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.WaterAlertDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IWaterAlertService
    {
        Task<PaginatedList<WaterAlertResponseDTO>> GetAllWaterAlertAsync( int pageIndex = 1, int pageSize = 10);
        Task<WaterAlertResponseDTO?> GetByIdAsync(int id);
        Task<WaterAlertResponseDTO> CreateAsync(int userId, WaterAlertRequestDTO dto);
        Task<bool> ResolveAsync(int id, int userId);
        Task<bool> DeleteAsync(int id);
    }
}
