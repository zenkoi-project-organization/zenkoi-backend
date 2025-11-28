using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class CreateIncidentWithDetailsDTO
    {
        [Required]
        public int IncidentTypeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string IncidentTitle { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public SeverityLevel? Severity { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public List<KoiIncidentRequestDTO>? AffectedKoiFish { get; set; }
        public List<PondIncidentRequestDTO>? AffectedPonds { get; set; }
    }
}
