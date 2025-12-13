using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentFilterDTO
    {
        public string? Search { get; set; }
        public int? IncidentTypeId { get; set; }
        public IncidentStatus? Status { get; set; }
        public int? ReportedByUserId { get; set; }
        public DateOnly? OccurredFrom { get; set; }
        public DateOnly? OccurredTo { get; set; }
        public int? PondId { get; set; }
        public int? KoiFishId { get; set; }
    }
}
