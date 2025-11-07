namespace Zenkoi.DAL.Entities;

public class WeeklyScheduleTemplateItem
{
    public int Id { get; set; }

    public int WeeklyScheduleTemplateId { get; set; }
    public WeeklyScheduleTemplate WeeklyScheduleTemplate { get; set; } = null!;

    public int TaskTemplateId { get; set; }
    public TaskTemplate TaskTemplate { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }
}
