using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.TaskTemplateDTOs;

public class TaskTemplateRequestDTO
{
    [Required(ErrorMessage = "Task name is required")]
    [MaxLength(200, ErrorMessage = "Task name cannot exceed 200 characters")]
    public string TaskName { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Default duration is required")]
    [Range(1, 1440, ErrorMessage = "Default duration must be between 1 and 1440 minutes")]
    public int DefaultDuration { get; set; }

    public bool IsRecurring { get; set; }

    [MaxLength(500, ErrorMessage = "Recurrence rule cannot exceed 500 characters")]
    public string? RecurrenceRule { get; set; }
}
