namespace Zenkoi.DAL.Entities;

public class WeeklyScheduleTemplate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<WeeklyScheduleTemplateItem> TemplateItems { get; set; } = new List<WeeklyScheduleTemplateItem>();
}
