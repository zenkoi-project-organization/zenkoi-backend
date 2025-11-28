using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentTypeDTOs
{
    public class IncidentTypeRequestDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public SeverityLevel DefaultSeverity { get; set; }

        public bool? AffectsBreeding { get; set; }
    }
}

