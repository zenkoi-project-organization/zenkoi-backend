using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;

namespace Zenkoi.BLL.DTOs.PondPacketFishDTOs
{
    public class PondPacketFishResponseDTO
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

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public PondBasicDTO Pond { get; set; }
        public BreedingProcessBasicDTO? BreedingProcess { get; set; }
        public PacketFishResponseDTO? PacketFish { get; set; }
    }
}
