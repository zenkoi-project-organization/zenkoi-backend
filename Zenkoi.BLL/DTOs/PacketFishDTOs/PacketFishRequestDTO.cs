using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.PacketFishDTOs
{
    public class PacketFishRequestDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name không vượt quá 100 kí tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description không quá 500 kí tự")]
        public string? Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MinSize của cá phải lớn hơn 0")]
        public double MinSize { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "MaxSize của cá phải lớn hơn 0")]
        public double MaxSize { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số cá trong mỗi packet phải lớn hơn 0")]
        public int FishPerPacket { get; set; } = 10;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "giá của packet phải lớn hơn không")]
        public decimal PricePerPacket { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "vui lòng chọn ít nhất một loại")]
        public List<int> VarietyIds { get; set; } = new List<int>();

        public List<string>? Images { get; set; }
        public List<string>? Videos { get; set; }

        [Required]
        public bool IsAvailable { get; set; } = true;
    }
}
