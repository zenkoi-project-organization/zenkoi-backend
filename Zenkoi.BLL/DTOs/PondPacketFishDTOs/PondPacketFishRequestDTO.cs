using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zenkoi.BLL.DTOs.PondPacketFishDTOs
{
    public class PondPacketFishRequestDTO
    {
        public int PondId { get; set; }
        public int PacketFishId { get; set; }
        public int BreedingProcessId { get; set; }
    }
}
