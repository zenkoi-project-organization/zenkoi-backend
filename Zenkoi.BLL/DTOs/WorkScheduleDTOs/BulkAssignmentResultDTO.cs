namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class BulkAssignmentResultDTO
{
    public int TotalAssignments { get; set; }
    public int SuccessfulAssignments { get; set; }
    public int FailedAssignments { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
