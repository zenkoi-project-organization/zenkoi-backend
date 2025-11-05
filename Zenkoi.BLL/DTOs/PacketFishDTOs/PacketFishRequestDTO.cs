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

/*      [Required]
        [Range(0, int.MaxValue, ErrorMessage = "StockQuantity cannot be negative")]
        public int StockQuantity { get; set; } = 0;*/

        [Required]
        public FishSize Size { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "AgeMonths must be greater than 0")]
        public decimal AgeMonths { get; set; }

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}
