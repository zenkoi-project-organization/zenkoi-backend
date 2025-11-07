using Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;

namespace Zenkoi.BLL.Services.Interfaces;

public interface IWeeklyScheduleTemplateService
{
    Task<WeeklyScheduleTemplateResponseDTO> CreateTemplateAsync(WeeklyScheduleTemplateRequestDTO dto);
    Task<WeeklyScheduleTemplateResponseDTO> GetTemplateByIdAsync(int id);
    Task<List<WeeklyScheduleTemplateResponseDTO>> GetAllTemplatesAsync();
    Task<WeeklyScheduleTemplateResponseDTO> UpdateTemplateAsync(int id, WeeklyScheduleTemplateRequestDTO dto);
    Task<bool> DeleteTemplateAsync(int id);
    Task<List<WorkScheduleResponseDTO>> GenerateWorkSchedulesFromTemplateAsync(GenerateScheduleRequestDTO request);
}
