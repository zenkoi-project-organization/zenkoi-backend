using System;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.DAL.Entities
{
    public class PondPacketFish
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public int PacketFishId { get; set; }
        public int BreedingProcessId { get; set; }

        public int AvailableQuantity { get; set; }
        public int SoldQuantity { get; set; }

        public int? TransferredFromId { get; set; }
        public int? TransferredToId { get; set; }
        public DateTime? TransferredAt { get; set; }
        public string? TransferReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public BreedingProcess BreedingProcess { get; set; }
        public PacketFish PacketFish { get; set; }
        public Pond Pond { get; set; }
        public PondPacketFish? TransferredFrom { get; set; }
        public PondPacketFish? TransferredTo { get; set; }
    }
}
