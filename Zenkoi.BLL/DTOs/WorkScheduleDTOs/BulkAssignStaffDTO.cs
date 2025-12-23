using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class BulkAssignStaffDTO
{
    [Required]
    public List<int> WorkScheduleIds { get; set; } = new List<int>();

    [Required]
    public List<int> StaffIds { get; set; } = new List<int>();
}
