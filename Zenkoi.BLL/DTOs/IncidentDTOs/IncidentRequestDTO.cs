using System;
using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class IncidentRequestDTO
    {
        [Required]
        public int IncidentTypeId { get; set; }
        [Required]
        [MaxLength(200)]
        public string IncidentTitle { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
