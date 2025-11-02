using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class WorkScheduleResponseDTO
{
    public int Id { get; set; }
    public int TaskTemplateId { get; set; }
    public string TaskTemplateName { get; set; } = string.Empty;
    public DateOnly ScheduledDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public WorkTaskStatus Status { get; set; }
    public string? Notes { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public TaskTemplateResponseDTO? TaskTemplate { get; set; }
    public List<StaffAssignmentResponseDTO> StaffAssignments { get; set; } = new();
    public List<PondAssignmentResponseDTO> PondAssignments { get; set; } = new();
}

public class StaffAssignmentResponseDTO
{
    public int WorkScheduleId { get; set; }
    public int StaffId { get; set; }
    public string StaffName { get; set; } = string.Empty;
    public string? CompletionNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class PondAssignmentResponseDTO
{
    public int WorkScheduleId { get; set; }
    public int PondId { get; set; }
    public string PondName { get; set; } = string.Empty;
}
