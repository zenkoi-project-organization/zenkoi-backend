using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class WeeklyScheduleTemplateItemDTO
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "TaskTemplateId must be greater than 0")]
    public int TaskTemplateId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; }
}
