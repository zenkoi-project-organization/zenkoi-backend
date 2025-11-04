using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPacketFishService
    {
        Task<PacketFishResponseDTO> CreatePacketFishAsync(PacketFishRequestDTO dto);
        Task<PacketFishResponseDTO> GetPacketFishByIdAsync(int id);
        Task<PaginatedList<PacketFishResponseDTO>> GetAllPacketFishesAsync(PacketFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10);
        Task<PacketFishResponseDTO> UpdatePacketFishAsync(int id, PacketFishUpdateDTO dto);
        Task<bool> DeletePacketFishAsync(int id);
        Task<PaginatedList<PacketFishResponseDTO>> GetAvailablePacketFishesAsync(int pageIndex = 1, int pageSize = 10);
        Task<PaginatedList<PacketFishResponseDTO>> GetPacketFishesBySizeAsync(FishSize size, int pageIndex = 1, int pageSize = 10);
        Task<PaginatedList<PacketFishResponseDTO>> GetPacketFishesByPriceRangeAsync(decimal minPrice, decimal maxPrice ,int pageIndex = 1, int pageSize = 10);
    }
}
