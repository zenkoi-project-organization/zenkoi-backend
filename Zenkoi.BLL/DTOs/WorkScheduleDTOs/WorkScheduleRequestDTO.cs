using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class WorkScheduleRequestDTO
{
    [Required(ErrorMessage = "Task template ID is required")]
    public int TaskTemplateId { get; set; }

    [Required(ErrorMessage = "Scheduled date is required")]
    public DateOnly ScheduledDate { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public TimeOnly StartTime { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }

    public List<int> StaffIds { get; set; } = new();

    public List<int> PondIds { get; set; } = new();
}
