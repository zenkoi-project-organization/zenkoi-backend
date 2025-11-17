using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;


public class CompleteStaffAssignmentDTO
{
    [MaxLength(1000, ErrorMessage = "Completion notes cannot exceed 1000 characters")]
    public string? CompletionNotes { get; set; }
}
