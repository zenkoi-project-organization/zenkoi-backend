using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class KoiIncidentRequestDTO
    {
        [Required(ErrorMessage = "ID cá Koi là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID cá Koi không hợp lệ")]
        public int KoiFishId { get; set; }

        [Required(ErrorMessage = "Tình trạng sức khỏe là bắt buộc")]
        public HealthStatus AffectedStatus { get; set; } = HealthStatus.Warning;

        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Triệu chứng phải có từ 5-1000 ký tự")]
        public string? SpecificSymptoms { get; set; }

        public bool RequiresTreatment { get; set; }

        public bool IsIsolated { get; set; }

        [Required(ErrorMessage = "Thời gian bắt đầu ảnh hưởng là bắt buộc")]
        public DateTime AffectedFrom { get; set; } = DateTime.UtcNow;

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Ghi chú điều trị phải có từ 10-2000 ký tự")]
        public string? TreatmentNotes { get; set; }
    }
}
