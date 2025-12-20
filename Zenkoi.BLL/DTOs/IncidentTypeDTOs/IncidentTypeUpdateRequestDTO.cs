using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentTypeDTOs
{
    public class IncidentTypeUpdateRequestDTO
    {
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên loại sự cố phải có từ 3-200 ký tự")]
        public string? Name { get; set; }

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mô tả phải có từ 10-2000 ký tự")]
        public string? Description { get; set; }

        public SeverityLevel? DefaultSeverity { get; set; }

        public bool? AffectsBreeding { get; set; }
    }
}

