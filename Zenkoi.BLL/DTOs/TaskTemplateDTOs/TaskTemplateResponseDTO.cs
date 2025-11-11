namespace Zenkoi.BLL.DTOs.TaskTemplateDTOs;

public class TaskTemplateResponseDTO
{
    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DefaultDuration { get; set; }
    public string? NotesTask { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
