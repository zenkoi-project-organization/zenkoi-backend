using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class EnrollFromVideoRequestDTO
    {
        [Required(ErrorMessage = "KoiFishId là bắt buộc")]
        public int KoiFishId { get; set; }

        [Required(ErrorMessage = "VideoUrl là bắt buộc")]
        public string VideoUrl { get; set; } = string.Empty;

        public int NumFrames { get; set; } = 15;

        public bool Override { get; set; } = false;
    }
}
