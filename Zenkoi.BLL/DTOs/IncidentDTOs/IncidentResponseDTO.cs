using System;
using System.Collections.Generic;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentResponseDTO
    {
        public int Id { get; set; }
        public int IncidentTypeId { get; set; }
        public string IncidentTypeName { get; set; }
        public string IncidentTitle { get; set; }
        public string Description { get; set; }
        public SeverityLevel Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int ReportedByUserId { get; set; }
        public string ReportedByUserName { get; set; }
        public int? ResolvedByUserId { get; set; }
        public string? ResolvedByUserName { get; set; }
        public string? ResolutionNotes { get; set; }
        public List<KoiIncidentSimpleDTO> KoiIncidents { get; set; }
        public List<PondIncidentSimpleDTO> PondIncidents { get; set; }
    }

    public class KoiIncidentSimpleDTO
    {
        public int Id { get; set; }
        public int KoiFishId { get; set; }
        public string KoiFishRFID { get; set; }
        public HealthStatus AffectedStatus { get; set; }
        public string SpecificSymptoms { get; set; }
        public bool RequiresTreatment { get; set; }
        public bool IsIsolated { get; set; }
    }

    public class PondIncidentSimpleDTO
    {
        public int Id { get; set; }
        public int PondId { get; set; }
        public string PondName { get; set; }
        public string EnvironmentalChanges { get; set; }
        public bool RequiresWaterChange { get; set; }
        public int? FishDiedCount { get; set; }
    }

    public class IncidentSimpleDTO
    {
        public int Id { get; set; }
        public int IncidentTypeId { get; set; }
        public string IncidentTypeName { get; set; }
        public string IncidentTitle { get; set; }
        public string Description { get; set; }
        public SeverityLevel Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime OccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ResolutionNotes { get; set; }
    }
}
