using Zenkoi.DAL.Enums;

namespace Zenkoi.DAL.Entities;

public class WorkSchedule
{
    public int Id { get; set; }
    public int TaskTemplateId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Pending;
    public string? Notes { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public TaskTemplate TaskTemplate { get; set; } = null!;
    public ApplicationUser Creator { get; set; } = null!;
    public ICollection<StaffAssignment> StaffAssignments { get; set; } = new List<StaffAssignment>();
    public ICollection<PondAssignment> PondAssignments { get; set; } = new List<PondAssignment>();
}
