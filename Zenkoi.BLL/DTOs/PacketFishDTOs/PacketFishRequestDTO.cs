using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PacketFishDTOs
{
    public class PacketFishRequestDTO
    {
        [Required]
        [StringLength(200, ErrorMessage = "Tên gói không vượt quá 200 kí tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không quá 500 kí tự")]
        public string? Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MinSize của gói cá phải lớn hơn 0")]
        public double MinSize { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MaxSize của gói cá phải lớn hơn 0")]
        public double MaxSize { get; set; }

        [Required]
        [Range(2, int.MaxValue, ErrorMessage = "Số cá trong mỗi packet phải lớn hơn 1")]
        public int FishPerPacket { get; set; } = 10;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá của packet phải lớn hơn không")]
        public decimal PricePerPacket { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public List<int> VarietyIds { get; set; } = new List<int>();

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}
