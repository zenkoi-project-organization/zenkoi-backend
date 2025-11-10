using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class UpdateStatusDto
    {
        [Required]
        public IncidentStatus Status { get; set; }
    }
}
