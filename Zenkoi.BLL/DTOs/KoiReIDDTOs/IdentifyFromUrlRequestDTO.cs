using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class IdentifyFromUrlRequestDTO
    {
        [Required]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string ImageUrl { get; set; }

        [Range(1, 20, ErrorMessage = "TopK phải từ 1 đến 20")]
        public int TopK { get; set; } = 5;

        [Range(0.0, 1.0, ErrorMessage = "Threshold phải từ 0.0 đến 1.0")]
        public decimal Threshold { get; set; } = 0.15m;
    }
}
