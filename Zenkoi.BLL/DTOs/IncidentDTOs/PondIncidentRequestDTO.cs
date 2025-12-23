using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class PondIncidentRequestDTO
    {
        [Required(ErrorMessage = "ID hồ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID hồ không hợp lệ")]
        public int PondId { get; set; }

        [StringLength(2000, ErrorMessage = "Thay đổi môi trường không được vượt quá 2000 ký tự")]
        public string? EnvironmentalChanges { get; set; }

        public bool RequiresWaterChange { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số cá chết không được âm")]
        public int? FishDiedCount { get; set; }

        [StringLength(2000, ErrorMessage = "Hành động khắc phục không được vượt quá 2000 ký tự")]
        public string? CorrectiveActions { get; set; }

        [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
        public string? Notes { get; set; }
    }
}
