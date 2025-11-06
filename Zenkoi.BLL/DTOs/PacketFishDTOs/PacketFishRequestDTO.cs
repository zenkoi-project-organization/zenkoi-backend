using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PacketFishDTOs
{
    public class PacketFishRequestDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "FishPerPacket must be at least 1")]
        public int FishPerPacket { get; set; } = 10;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "PricePerPacket must be greater than 0")]
        public decimal PricePerPacket { get; set; }

        [Required]
        public FishSize Size { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one variety is required")]
        public List<int> VarietyIds { get; set; } = new List<int>();

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}
