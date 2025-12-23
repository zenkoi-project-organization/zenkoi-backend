using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentRequestDTO
    {
        [Required(ErrorMessage = "Loại sự cố là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Loại sự cố không hợp lệ")]
        public int IncidentTypeId { get; set; }

        [Required(ErrorMessage = "Tiêu đề sự cố là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string IncidentTitle { get; set; }

        [Required(ErrorMessage = "Mô tả sự cố là bắt buộc")]
        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Thời gian xảy ra sự cố là bắt buộc")]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        public List<string>? ReportImages { get; set; }
    }
}
