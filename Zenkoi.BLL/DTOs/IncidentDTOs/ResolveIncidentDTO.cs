using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.IncidentDTOs
{
    public class ResolveIncidentDTO
    {
        [Required(ErrorMessage = "Ghi chú giải quyết không được để trống")]
        [StringLength(2000, ErrorMessage = "Ghi chú giải quyết phải ít hơn 2000 ký tự")]
        public string ResolutionNotes { get; set; }

        public List<string>? ResolutionImages { get; set; }
    }
}
