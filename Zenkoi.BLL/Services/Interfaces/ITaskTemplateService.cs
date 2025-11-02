using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces;

public interface ITaskTemplateService
{
    Task<PaginatedList<TaskTemplateResponseDTO>> GetAllTaskTemplatesAsync(
        TaskTemplateFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10);

    Task<TaskTemplateResponseDTO> GetTaskTemplateByIdAsync(int id);

    Task<TaskTemplateResponseDTO> CreateTaskTemplateAsync(TaskTemplateRequestDTO dto);

    Task<TaskTemplateResponseDTO> UpdateTaskTemplateAsync(int id, TaskTemplateRequestDTO dto);

    Task<bool> DeleteTaskTemplateAsync(int id);

    Task<bool> RestoreTaskTemplateAsync(int id);
}
