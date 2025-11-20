using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.KoiFavoriteDTOs
{
    public class KoiFavoriteRequestDTO
    {
        [Required(ErrorMessage = "KoiFishId is required.")]
        public int KoiFishId { get; set; }
    }
}
