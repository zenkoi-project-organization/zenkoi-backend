using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class UpdateWorkScheduleStatusDTO
{
    [Required(ErrorMessage = "Status is required")]
    public WorkTaskStatus Status { get; set; }

    [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string? Notes { get; set; }
}
