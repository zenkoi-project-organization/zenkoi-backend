using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PacketFishDTOs
{
    public class PacketFishResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public FishSize Size { get; set; }
        public decimal AgeMonths { get; set; }
        public string? Images { get; set; }
        public string? Video { get; set; }
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
        public int PacketFishId { get; set; }
        public string PacketFishName { get; set; } = string.Empty;
    }
}
