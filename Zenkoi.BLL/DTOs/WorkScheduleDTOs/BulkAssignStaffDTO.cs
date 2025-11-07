using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class BulkAssignStaffDTO
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one work schedule must be selected")]
    public List<int> WorkScheduleIds { get; set; } = new List<int>();

    [Required]
    [MinLength(1, ErrorMessage = "At least one staff member must be selected")]
    public List<int> StaffIds { get; set; } = new List<int>();
}
