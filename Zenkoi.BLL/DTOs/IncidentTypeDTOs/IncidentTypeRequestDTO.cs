using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentTypeDTOs
{
    public class IncidentTypeRequestDTO
    {
        [Required(ErrorMessage = "Tên loại sự cố là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên loại sự cố không được vượt quá 200 ký tự")]
        public string Name { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Mức độ nghiêm trọng mặc định là bắt buộc")]
        public SeverityLevel DefaultSeverity { get; set; }

        public bool? AffectsBreeding { get; set; }
    }
}

