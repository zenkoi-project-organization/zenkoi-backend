namespace Zenkoi.DAL.Entities;

public class StaffAssignment
{
    public int WorkScheduleId { get; set; }
    public int StaffId { get; set; }
    public string? CompletionNotes { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public WorkSchedule WorkSchedule { get; set; } = null!;
    public ApplicationUser Staff { get; set; } = null!;
}