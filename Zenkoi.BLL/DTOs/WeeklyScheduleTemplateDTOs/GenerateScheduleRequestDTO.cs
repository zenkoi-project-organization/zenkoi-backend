using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class GenerateScheduleRequestDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "WeeklyScheduleTemplateId must be greater than 0")]
    public int WeeklyScheduleTemplateId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }
}
