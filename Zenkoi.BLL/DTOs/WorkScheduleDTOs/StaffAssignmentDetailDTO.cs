using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class StaffAssignmentDetailDTO
{
    public int WorkScheduleId { get; set; }
    public int StaffId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletionNotes { get; set; }
    public bool IsCompleted { get; set; }

    public DateOnly ScheduledDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public WorkTaskStatus Status { get; set; }
    public string? WorkScheduleNotes { get; set; }

    public int TaskTemplateId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? TaskDescription { get; set; }
    public int DefaultDuration { get; set; }
    public string? TaskNotes { get; set; }

    public List<string> PondNames { get; set; } = new();

    public int TotalStaffAssigned { get; set; }
    public int TotalStaffCompleted { get; set; }
}
