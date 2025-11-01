using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class PondPacketFish
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public int PacketFishId { get; set; }
        public int BreedingProcessId { get; set; }
        
        // stock inside the pond
        public int Quantity { get; set; }
        public BreedingProcess BreedingProcess { get; set; }
        public PacketFish PacketFish { get; set; }
        public Pond Pond { get; set; }
    }
}
