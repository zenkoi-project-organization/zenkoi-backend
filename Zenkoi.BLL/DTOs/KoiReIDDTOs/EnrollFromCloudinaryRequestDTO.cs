using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.KoiReIDDTOs
{
    public class EnrollFromCloudinaryRequestDTO
    {
        [Required]
        public int KoiFishId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Phải có ít nhất 1 URL")]
        public List<string> CloudinaryUrls { get; set; }

        public bool Override { get; set; } = false;
    }
}
