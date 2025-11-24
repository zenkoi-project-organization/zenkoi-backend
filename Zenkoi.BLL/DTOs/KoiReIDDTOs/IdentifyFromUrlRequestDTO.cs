using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class IdentifyFromUrlRequestDTO
    {
        [Required]
        [Url(ErrorMessage = "URL không hợp lệ")]
        public string ImageUrl { get; set; }
    }
}
