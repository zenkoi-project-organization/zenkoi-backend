using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class WeeklyScheduleTemplateRequestDTO
{
    [Required(ErrorMessage = "Tên mẫu lịch làm việc là bắt buộc")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Tên phải có từ 3-500 ký tự")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, MinimumLength = 5, ErrorMessage = "Mô tả phải có từ 5-500 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ít nhất một mục mẫu lịch là bắt buộc")]
    [MinLength(1, ErrorMessage = "Ít nhất một mục mẫu lịch là bắt buộc")]
    public List<WeeklyScheduleTemplateItemDTO> TemplateItems { get; set; } = new List<WeeklyScheduleTemplateItemDTO>();
}
