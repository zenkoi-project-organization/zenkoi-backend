using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class UpdateStatusDto
    {
        [Required]
        public IncidentStatus Status { get; set; }

        [MaxLength(2000)]
        public string? ResolutionNotes { get; set; }

        public List<string>? ResolutionImages { get; set; }
    }
}
