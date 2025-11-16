using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PacketFishDTOs
{
    public class PacketFishResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int FishPerPacket { get; set; }
        public decimal PricePerPacket { get; set; }
        public int StockQuantity { get; set; }
        public string Size { get; set; }
        public decimal AgeMonths { get; set; }
        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<VarietyPacketFishResponseDTO> VarietyPacketFishes { get; set; } = new List<VarietyPacketFishResponseDTO>();
    }

    public class VarietyPacketFishResponseDTO
    {
        public int Id { get; set; }
        public int VarietyId { get; set; }
        public string VarietyName { get; set; } = string.Empty;
    }
}
