using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentUpdateRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Loại sự cố không hợp lệ")]
        public int? IncidentTypeId { get; set; }

        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải có từ 5-200 ký tự")]
        public string? IncidentTitle { get; set; }

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mô tả phải có từ 10-2000 ký tự")]
        public string? Description { get; set; }

        public IncidentStatus? Status { get; set; }

        public DateTime? OccurredAt { get; set; }

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Ghi chú giải quyết phải có từ 10-2000 ký tự")]
        public string? ResolutionNotes { get; set; }

        public List<string>? ReportImages { get; set; }

        // Update affected Koi and Ponds
        public List<KoiIncidentRequestDTO>? AffectedKoiFish { get; set; }
        public List<PondIncidentRequestDTO>? AffectedPonds { get; set; }
    }
}
