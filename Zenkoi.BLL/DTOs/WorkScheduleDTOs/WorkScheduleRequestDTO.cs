using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WorkScheduleDTOs;

public class WorkScheduleRequestDTO
{
    [Required(ErrorMessage = "Mẫu công việc là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Mẫu công việc không hợp lệ")]
    public int TaskTemplateId { get; set; }

    [Required(ErrorMessage = "Ngày lên lịch là bắt buộc")]
    public DateOnly ScheduledDate { get; set; }

    [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc")]
    public TimeOnly StartTime { get; set; }

    public string? Notes { get; set; }

    [MinLength(1, ErrorMessage = "Phải chỉ định ít nhất 1 nhân viên")]
    public List<int> StaffIds { get; set; } = new();

    [MinLength(1, ErrorMessage = "Phải chỉ định ít nhất 1 hồ")]
    public List<int> PondIds { get; set; } = new();
}
