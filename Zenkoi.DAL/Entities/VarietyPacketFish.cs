using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class VarietyPacketFish
    {
        public int Id { get; set; }
        public int VarietyId { get; set; }
        public Variety Variety { get; set; }
        public int PacketFishId { get; set; }
        public PacketFish PacketFish { get; set; }
        public int Count { get; set; }
    }
}
