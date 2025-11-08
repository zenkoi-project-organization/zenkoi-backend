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

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? NotesTask { get; set; }
}
