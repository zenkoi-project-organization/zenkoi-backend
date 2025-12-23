using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;

public class WeeklyScheduleTemplateRequestDTO
{
    [Required(ErrorMessage = "Tên mẫu lịch làm việc là bắt buộc")]
    [StringLength(500, ErrorMessage = "Tên không được vượt quá 500 ký tự")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ít nhất một mục mẫu lịch là bắt buộc")]
    public List<WeeklyScheduleTemplateItemDTO> TemplateItems { get; set; } = new List<WeeklyScheduleTemplateItemDTO>();
}
