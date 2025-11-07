using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class WeeklyScheduleTemplateRequestDTO
{
    [Required]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one template item is required")]
    public List<WeeklyScheduleTemplateItemDTO> TemplateItems { get; set; } = new List<WeeklyScheduleTemplateItemDTO>();
}
