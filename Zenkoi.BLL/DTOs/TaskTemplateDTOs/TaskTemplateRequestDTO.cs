using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.TaskTemplateDTOs;

public class TaskTemplateRequestDTO
{
    [Required(ErrorMessage = "Tên công việc là bắt buộc")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên công việc phải có từ 3-200 ký tự")]
    public string TaskName { get; set; } = string.Empty;

    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Mô tả phải có từ 10-1000 ký tự")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Thời lượng mặc định là bắt buộc")]
    [Range(1, 1440, ErrorMessage = "Thời lượng phải từ 1-1440 phút (tối đa 24 giờ)")]
    public int DefaultDuration { get; set; }

    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Ghi chú phải có từ 5-1000 ký tự")]
    public string? NotesTask { get; set; }
}
