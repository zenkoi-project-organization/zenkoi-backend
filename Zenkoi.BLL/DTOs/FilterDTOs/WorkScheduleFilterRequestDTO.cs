using Zenkoi.BLL.Helpers.Validations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs;

public class WorkScheduleFilterRequestDTO
{
    public string? Search { get; set; }
    public int? TaskTemplateId { get; set; }
    public WorkTaskStatus? Status { get; set; }
    public int? StaffId { get; set; }
    public int? PondId { get; set; }
    public Role? StaffRole { get; set; }

    [DateRangeValidation(nameof(ScheduledDateTo))]
    public DateOnly? ScheduledDateFrom { get; set; }

    public DateOnly? ScheduledDateTo { get; set; }
    public int? CreatedBy { get; set; }
}
