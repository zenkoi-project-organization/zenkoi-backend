using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;
using Zenkoi.DAL.Paging;

namespace Zenkoi.BLL.Services.Interfaces;

public interface IWorkScheduleService
{
    Task<PaginatedList<WorkScheduleResponseDTO>> GetAllWorkSchedulesAsync(
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10);

    Task<WorkScheduleResponseDTO> GetWorkScheduleByIdAsync(int id);

    Task<WorkScheduleResponseDTO> CreateWorkScheduleAsync(WorkScheduleRequestDTO dto, int createdBy);

    Task<WorkScheduleResponseDTO> UpdateWorkScheduleAsync(int id, WorkScheduleRequestDTO dto);

    Task<WorkScheduleResponseDTO> UpdateWorkScheduleStatusAsync(int id, UpdateWorkScheduleStatusDTO dto);

    Task<bool> DeleteWorkScheduleAsync(int id);

    Task<PaginatedList<WorkScheduleResponseDTO>> GetWorkSchedulesByStaffIdAsync(
        int staffId,
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10);

    Task<PaginatedList<WorkScheduleResponseDTO>> GetWorkSchedulesByPondIdAsync(
        int pondId,
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10);

    Task<BulkAssignmentResultDTO> BulkAssignStaffAsync(BulkAssignStaffDTO dto);
}
