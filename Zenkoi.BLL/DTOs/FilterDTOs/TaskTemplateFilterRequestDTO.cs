namespace Zenkoi.BLL.DTOs.FilterDTOs;

public class TaskTemplateFilterRequestDTO
{
    public string? Search { get; set; }
    public bool? IsRecurring { get; set; }
    public bool? IsDeleted { get; set; }
}
