using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IPacketFishService
    {
        Task<PacketFishResponseDTO> CreatePacketFishAsync(PacketFishRequestDTO dto);
        Task<PacketFishResponseDTO> GetPacketFishByIdAsync(int id);
        Task<IEnumerable<PacketFishResponseDTO>> GetAllPacketFishesAsync(QueryOptions<PacketFish>? queryOptions = null);
        Task<PacketFishResponseDTO> UpdatePacketFishAsync(int id, PacketFishUpdateDTO dto);
        Task<bool> DeletePacketFishAsync(int id);
        Task<IEnumerable<PacketFishResponseDTO>> GetAvailablePacketFishesAsync();
        Task<IEnumerable<PacketFishResponseDTO>> GetPacketFishesBySizeAsync(FishSize size);
        Task<IEnumerable<PacketFishResponseDTO>> GetPacketFishesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
