using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class PondPacketFish
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public Pond Pond { get; set; }

        public int PacketFishId { get; set; }
        public PacketFish PacketFish { get; set; }

        // stock inside the pond
        public int Quantity { get; set; }
    }
}
