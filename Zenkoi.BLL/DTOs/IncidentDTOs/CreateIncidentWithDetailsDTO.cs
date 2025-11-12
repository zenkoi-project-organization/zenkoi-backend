using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class CreateIncidentWithDetailsDTO
    {
        // Incident base info
        [Required]
        public int IncidentTypeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string IncidentTitle { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public SeverityLevel Severity { get; set; } = SeverityLevel.Medium;

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;

        // Affected Koi Fish
        public List<KoiIncidentRequestDTO>? AffectedKoiFish { get; set; }

        // Affected Ponds
        public List<PondIncidentRequestDTO>? AffectedPonds { get; set; }
    }
}
