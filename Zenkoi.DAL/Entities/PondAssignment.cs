namespace Zenkoi.DAL.Entities;

public class PondAssignment
{
    public int WorkScheduleId { get; set; }
    public int PondId { get; set; }

    public WorkSchedule WorkSchedule { get; set; } = null!;
    public Pond Pond { get; set; } = null!;
}