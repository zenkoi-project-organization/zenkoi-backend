using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class ResolveIncidentDTO
    {
        [Required(ErrorMessage = "Ghi chú giải quyết không được để trống")]
        [MaxLength(2000)]
        public string ResolutionNotes { get; set; }
    }
}
