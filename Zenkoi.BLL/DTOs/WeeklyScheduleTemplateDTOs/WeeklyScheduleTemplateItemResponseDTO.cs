using Zenkoi.BLL.DTOs.TaskTemplateDTOs;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class WeeklyScheduleTemplateItemResponseDTO
{
    public int Id { get; set; }
    public int TaskTemplateId { get; set; }
    public TaskTemplateResponseDTO? TaskTemplate { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
