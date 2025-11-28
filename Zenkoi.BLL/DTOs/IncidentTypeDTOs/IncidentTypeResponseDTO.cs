using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.IncidentTypeDTOs
{
    public class IncidentTypeResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public SeverityLevel DefaultSeverity { get; set; }
        public bool? AffectsBreeding { get; set; }
    }
}

