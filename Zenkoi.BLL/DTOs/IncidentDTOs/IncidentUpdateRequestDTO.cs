using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentUpdateRequestDTO
    {
        public int? IncidentTypeId { get; set; }
        [MaxLength(200)]
        public string? IncidentTitle { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; }
        public SeverityLevel? Severity { get; set; }
        public IncidentStatus? Status { get; set; }
        public DateTime? OccurredAt { get; set; }
        [MaxLength(2000)]
        public string? ResolutionNotes { get; set; }
    }
}
