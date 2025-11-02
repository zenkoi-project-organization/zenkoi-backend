namespace Zenkoi.BLL.DTOs.TaskTemplateDTOs;

public class TaskTemplateResponseDTO
{
    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultDuration { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurrenceRule { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
