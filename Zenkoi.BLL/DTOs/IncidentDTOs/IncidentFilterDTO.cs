using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentFilterDTO
    {
        public string? Search { get; set; }
        public int? IncidentTypeId { get; set; }
        public SeverityLevel? Severity { get; set; }
        public IncidentStatus? Status { get; set; }
        public int? ReportedByUserId { get; set; }
        public DateTime? OccurredFrom { get; set; }
        public DateTime? OccurredTo { get; set; }
    }
}
