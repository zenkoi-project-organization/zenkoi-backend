using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondPacketFishDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPondPacketFishService
    {
        Task<PaginatedList<PondPacketFishResponseDTO>> GetAllPondPacketFishAsync(int pageIndex = 1, int pageSize = 10);
        Task<PondPacketFishResponseDTO> GetByIdAsync(int id);
        Task<PondPacketFishResponseDTO> CreateAsync(PondPacketFishRequestDTO dto);
        Task<bool> TranferPacket(int id, PondPacketFishUpdateRequestDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
