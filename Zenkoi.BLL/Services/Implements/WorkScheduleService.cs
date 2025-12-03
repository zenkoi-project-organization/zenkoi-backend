using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements;

public class WorkScheduleService : IWorkScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepoBase<WorkSchedule> _workScheduleRepo;
    private readonly IRepoBase<TaskTemplate> _taskTemplateRepo;
    private readonly IRepoBase<StaffAssignment> _staffAssignmentRepo;
    private readonly IRepoBase<PondAssignment> _pondAssignmentRepo;
    private readonly IRepoBase<ApplicationUser> _userRepo;
    private readonly IRepoBase<Pond> _pondRepo;

    public WorkScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _workScheduleRepo = _unitOfWork.GetRepo<WorkSchedule>();
        _taskTemplateRepo = _unitOfWork.GetRepo<TaskTemplate>();
        _staffAssignmentRepo = _unitOfWork.GetRepo<StaffAssignment>();
        _pondAssignmentRepo = _unitOfWork.GetRepo<PondAssignment>();
        _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
        _pondRepo = _unitOfWork.GetRepo<Pond>();
    }

    public async Task<List<WorkScheduleResponseDTO>> GetAllWorkSchedulesAsync(WorkScheduleFilterRequestDTO filter)
    {
        var queryBuilder = new QueryBuilder<WorkSchedule>()
            .WithTracking(false)
            .WithInclude(ws => ws.TaskTemplate)
            .WithInclude(ws => ws.Creator)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments);

        ApplyFilters(queryBuilder, filter);

        queryBuilder.WithOrderBy(q => q.OrderByDescending(ws => ws.ScheduledDate)
            .ThenByDescending(ws => ws.StartTime));

        var query = _workScheduleRepo.Get(queryBuilder.Build());
        var entities = await query
            .Include(ws => ws.StaffAssignments)
                .ThenInclude(sa => sa.Staff)
            .ToListAsync();

        await LoadNavigationPropertiesAsync(entities);

        return _mapper.Map<List<WorkScheduleResponseDTO>>(entities);
    }

    public async Task<WorkScheduleResponseDTO> GetWorkScheduleByIdAsync(int id)
    {
        var workSchedule = await _workScheduleRepo.GetSingleAsync(new QueryBuilder<WorkSchedule>()
            .WithPredicate(ws => ws.Id == id)
            .WithTracking(false)
            .WithInclude(ws => ws.TaskTemplate)
            .WithInclude(ws => ws.Creator)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments)
            .Build());

        if (workSchedule == null)
            throw new ArgumentException("Work schedule not found");

        await LoadNavigationPropertiesForSingleAsync(workSchedule);

        return _mapper.Map<WorkScheduleResponseDTO>(workSchedule);
    }

    public async Task<WorkScheduleResponseDTO> CreateWorkScheduleAsync(WorkScheduleRequestDTO dto, int createdBy)
    {
        var taskTemplate = await _taskTemplateRepo.GetByIdAsync(dto.TaskTemplateId);
        if (taskTemplate == null)
            throw new ArgumentException("Task template not found");

        if (taskTemplate.IsDeleted)
            throw new ArgumentException("Cannot create work schedule from deleted task template");

        foreach (var staffId in dto.StaffIds)
        {
            var staff = await _userRepo.GetByIdAsync(staffId);
            if (staff == null)
                throw new ArgumentException($"Staff with ID {staffId} not found");
        }

        foreach (var pondId in dto.PondIds)
        {
            var pond = await _pondRepo.GetByIdAsync(pondId);
            if (pond == null)
                throw new ArgumentException($"Pond with ID {pondId} not found");
        }

        var workSchedule = _mapper.Map<WorkSchedule>(dto);
        workSchedule.CreatedBy = createdBy;
        workSchedule.CreatedAt = DateTime.UtcNow;

        await _workScheduleRepo.CreateAsync(workSchedule);
        await _unitOfWork.SaveChangesAsync();

        foreach (var staffId in dto.StaffIds)
        {
            var staffAssignment = new StaffAssignment
            {
                WorkScheduleId = workSchedule.Id,
                StaffId = staffId
            };
            await _staffAssignmentRepo.CreateAsync(staffAssignment);
        }

        foreach (var pondId in dto.PondIds)
        {
            var pondAssignment = new PondAssignment
            {
                WorkScheduleId = workSchedule.Id,
                PondId = pondId
            };
            await _pondAssignmentRepo.CreateAsync(pondAssignment);
        }

        await _unitOfWork.SaveChangesAsync();

        return await GetWorkScheduleByIdAsync(workSchedule.Id);
    }

    public async Task<WorkScheduleResponseDTO> UpdateWorkScheduleAsync(int id, WorkScheduleRequestDTO dto)
    {
        var workSchedule = await _workScheduleRepo.GetSingleAsync(new QueryBuilder<WorkSchedule>()
            .WithPredicate(ws => ws.Id == id)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments)
            .Build());

        if (workSchedule == null)
            throw new ArgumentException("Work schedule not found");

        var taskTemplate = await _taskTemplateRepo.GetByIdAsync(dto.TaskTemplateId);
        if (taskTemplate == null)
            throw new ArgumentException("Task template not found");

        _mapper.Map(dto, workSchedule);
        workSchedule.UpdatedAt = DateTime.UtcNow;

        await _workScheduleRepo.UpdateAsync(workSchedule);

        var existingStaffAssignments = workSchedule.StaffAssignments.ToList();
        foreach (var assignment in existingStaffAssignments)
        {
            await _staffAssignmentRepo.DeleteAsync(assignment);
        }

        var existingPondAssignments = workSchedule.PondAssignments.ToList();
        foreach (var assignment in existingPondAssignments)
        {
            await _pondAssignmentRepo.DeleteAsync(assignment);
        }

        await _unitOfWork.SaveChangesAsync();

        foreach (var staffId in dto.StaffIds)
        {
            var staff = await _userRepo.GetByIdAsync(staffId);
            if (staff == null)
                throw new ArgumentException($"Staff with ID {staffId} not found");

            var staffAssignment = new StaffAssignment
            {
                WorkScheduleId = workSchedule.Id,
                StaffId = staffId
            };
            await _staffAssignmentRepo.CreateAsync(staffAssignment);
        }

        foreach (var pondId in dto.PondIds)
        {
            var pond = await _pondRepo.GetByIdAsync(pondId);
            if (pond == null)
                throw new ArgumentException($"Pond with ID {pondId} not found");

            var pondAssignment = new PondAssignment
            {
                WorkScheduleId = workSchedule.Id,
                PondId = pondId
            };
            await _pondAssignmentRepo.CreateAsync(pondAssignment);
        }

        await _unitOfWork.SaveChangesAsync();

        return await GetWorkScheduleByIdAsync(workSchedule.Id);
    }

    public async Task<WorkScheduleResponseDTO> UpdateWorkScheduleStatusAsync(int id, UpdateWorkScheduleStatusDTO dto)
    {
        var workSchedule = await _workScheduleRepo.GetByIdAsync(id);

        if (workSchedule == null)
            throw new ArgumentException("Work schedule not found");

        workSchedule.Status = dto.Status;
        if (!string.IsNullOrEmpty(dto.Notes))
        {
            workSchedule.Notes = dto.Notes;
        }
        workSchedule.UpdatedAt = DateTime.UtcNow;

        await _workScheduleRepo.UpdateAsync(workSchedule);
        await _unitOfWork.SaveChangesAsync();

        return await GetWorkScheduleByIdAsync(workSchedule.Id);
    }

    public async Task<bool> DeleteWorkScheduleAsync(int id)
    {
        var workSchedule = await _workScheduleRepo.GetSingleAsync(new QueryBuilder<WorkSchedule>()
            .WithPredicate(ws => ws.Id == id)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments)
            .Build());

        if (workSchedule == null)
            return false;

        foreach (var assignment in workSchedule.StaffAssignments)
        {
            await _staffAssignmentRepo.DeleteAsync(assignment);
        }

        foreach (var assignment in workSchedule.PondAssignments)
        {
            await _pondAssignmentRepo.DeleteAsync(assignment);
        }

        await _workScheduleRepo.DeleteAsync(workSchedule);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<PaginatedList<WorkScheduleResponseDTO>> GetWorkSchedulesByStaffIdAsync(
        int staffId,
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10)
    {
        filter.StaffId = staffId;
        return await GetWorkSchedulesWithPaginationAsync(filter, pageIndex, pageSize);
    }

    public async Task<PaginatedList<WorkScheduleResponseDTO>> GetWorkSchedulesByPondIdAsync(
        int pondId,
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10)
    {
        filter.PondId = pondId;
        return await GetWorkSchedulesWithPaginationAsync(filter, pageIndex, pageSize);
    }

    public async Task<List<WorkScheduleResponseDTO>> GetWorkSchedulesByUserIdAsync(
        int userId,
        WorkScheduleFilterRequestDTO filter)
    {
        filter.StaffId = userId;

        var queryBuilder = new QueryBuilder<WorkSchedule>()
            .WithTracking(false)
            .WithInclude(ws => ws.TaskTemplate)
            .WithInclude(ws => ws.Creator)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments);

        ApplyFilters(queryBuilder, filter);

        queryBuilder.WithOrderBy(q => q.OrderByDescending(ws => ws.ScheduledDate)
            .ThenByDescending(ws => ws.StartTime));

        var query = _workScheduleRepo.Get(queryBuilder.Build());
        var entities = await query
            .Include(ws => ws.StaffAssignments)
                .ThenInclude(sa => sa.Staff)
            .ToListAsync();

        await LoadNavigationPropertiesAsync(entities);

        return _mapper.Map<List<WorkScheduleResponseDTO>>(entities);
    }

    private async Task<PaginatedList<WorkScheduleResponseDTO>> GetWorkSchedulesWithPaginationAsync(
        WorkScheduleFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var queryBuilder = new QueryBuilder<WorkSchedule>()
            .WithTracking(false)
            .WithInclude(ws => ws.TaskTemplate)
            .WithInclude(ws => ws.Creator)
            .WithInclude(ws => ws.StaffAssignments)
            .WithInclude(ws => ws.PondAssignments);

        ApplyFilters(queryBuilder, filter);

        queryBuilder.WithOrderBy(q => q.OrderByDescending(ws => ws.ScheduledDate)
            .ThenByDescending(ws => ws.StartTime));

        var query = _workScheduleRepo.Get(queryBuilder.Build());
        query = query.Include(ws => ws.StaffAssignments)
                .ThenInclude(sa => sa.Staff);

        var paginatedEntities = await PaginatedList<WorkSchedule>.CreateAsync(query, pageIndex, pageSize);

        await LoadNavigationPropertiesAsync(paginatedEntities);

        var resultDto = _mapper.Map<List<WorkScheduleResponseDTO>>(paginatedEntities);

        return new PaginatedList<WorkScheduleResponseDTO>(
            resultDto,
            paginatedEntities.TotalItems,
            pageIndex,
            pageSize);
    }

    private void ApplyFilters(QueryBuilder<WorkSchedule> queryBuilder, WorkScheduleFilterRequestDTO filter)
    {
        if (!string.IsNullOrEmpty(filter.Search))
        {
            queryBuilder.WithPredicate(ws =>
                (ws.TaskTemplate != null && ws.TaskTemplate.TaskName.Contains(filter.Search)) ||
                (ws.Notes != null && ws.Notes.Contains(filter.Search)));
        }

        if (filter.TaskTemplateId.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.TaskTemplateId == filter.TaskTemplateId.Value);
        }

        if (filter.Status.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.Status == filter.Status.Value);
        }

        if (filter.StaffId.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.StaffAssignments.Any(sa => sa.StaffId == filter.StaffId.Value));
        }

        if (filter.StaffRole.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.StaffAssignments.Any(sa => sa.Staff.Role == filter.StaffRole.Value));
        }

        if (filter.PondId.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.PondAssignments.Any(pa => pa.PondId == filter.PondId.Value));
        }

        if (filter.ScheduledDateFrom.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.ScheduledDate >= filter.ScheduledDateFrom.Value);
        }

        if (filter.ScheduledDateTo.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.ScheduledDate <= filter.ScheduledDateTo.Value);
        }

        if (filter.CreatedBy.HasValue)
        {
            queryBuilder.WithPredicate(ws => ws.CreatedBy == filter.CreatedBy.Value);
        }
    }

    private async Task LoadNavigationPropertiesAsync(IEnumerable<WorkSchedule> workSchedules)
    {
        foreach (var ws in workSchedules)
        {
            await LoadNavigationPropertiesForSingleAsync(ws);
        }
    }

    private async Task LoadNavigationPropertiesForSingleAsync(WorkSchedule workSchedule)
    {
        if (workSchedule.StaffAssignments != null && workSchedule.StaffAssignments.Any())
        {
            foreach (var sa in workSchedule.StaffAssignments)
            {
                sa.Staff = await _userRepo.GetByIdAsync(sa.StaffId);
            }
        }

        if (workSchedule.PondAssignments != null && workSchedule.PondAssignments.Any())
        {
            foreach (var pa in workSchedule.PondAssignments)
            {
                pa.Pond = await _pondRepo.GetByIdAsync(pa.PondId);
            }
        }
    }

    public async Task<BulkAssignmentResultDTO> BulkAssignStaffAsync(BulkAssignStaffDTO dto)
    {
        var result = new BulkAssignmentResultDTO();

        try
        {
            await _unitOfWork.BeginTransactionAsync();

            foreach (var workScheduleId in dto.WorkScheduleIds)
            {
                var workSchedule = await _workScheduleRepo.GetByIdAsync(workScheduleId);
                if (workSchedule == null)
                {
                    result.Errors.Add($"Work schedule with ID {workScheduleId} not found");
                    result.FailedAssignments += dto.StaffIds.Count;
                    continue;
                }

                foreach (var staffId in dto.StaffIds)
                {
                    result.TotalAssignments++;

                    var staff = await _userRepo.GetByIdAsync(staffId);
                    if (staff == null)
                    {
                        result.Errors.Add($"Staff with ID {staffId} not found");
                        result.FailedAssignments++;
                        continue;
                    }

                    var existingAssignment = await _staffAssignmentRepo.GetSingleAsync(new QueryBuilder<StaffAssignment>()
                        .WithPredicate(sa => sa.WorkScheduleId == workScheduleId && sa.StaffId == staffId)
                        .Build());

                    if (existingAssignment != null)
                    {
                        result.Errors.Add($"Staff {staffId} is already assigned to work schedule {workScheduleId}");
                        result.FailedAssignments++;
                        continue;
                    }

                    var staffAssignment = new StaffAssignment
                    {
                        WorkScheduleId = workScheduleId,
                        StaffId = staffId
                    };

                    await _staffAssignmentRepo.CreateAsync(staffAssignment);
                    result.SuccessfulAssignments++;
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollBackAsync();
            result.Errors.Add($"An error occurred during bulk assignment: {ex.Message}");
            throw;
        }
    }

    public async Task<WorkScheduleResponseDTO> CompleteStaffAssignmentAsync(
        int workScheduleId,
        int staffId,
        CompleteStaffAssignmentDTO dto)
    {
        var workSchedule = await _workScheduleRepo.Get(
            new QueryBuilder<WorkSchedule>()
                .WithTracking(true)
                .WithPredicate(ws => ws.Id == workScheduleId)
                .WithInclude(ws => ws.StaffAssignments)
                .Build())
            .FirstOrDefaultAsync();

        if (workSchedule == null)
            throw new ArgumentException("Work schedule not found");
      
        var staffAssignment = workSchedule.StaffAssignments
            .FirstOrDefault(sa => sa.StaffId == staffId);

        if (staffAssignment == null)
            throw new ArgumentException("Staff is not assigned to this work schedule");

        if (staffAssignment.CompletedAt != null)
            throw new InvalidOperationException("This assignment has already been marked as completed");

        staffAssignment.CompletedAt = DateTime.UtcNow;
        staffAssignment.CompletionNotes = dto.CompletionNotes;

        await _staffAssignmentRepo.UpdateAsync(staffAssignment);

        var allStaffCompleted = workSchedule.StaffAssignments.All(sa => sa.CompletedAt != null);

        if (allStaffCompleted && workSchedule.Status != DAL.Enums.WorkTaskStatus.Completed)
        {
            workSchedule.Status = DAL.Enums.WorkTaskStatus.Completed;
            workSchedule.UpdatedAt = DateTime.UtcNow;
            await _workScheduleRepo.UpdateAsync(workSchedule);
        }
        else if (workSchedule.Status == DAL.Enums.WorkTaskStatus.Pending)
        {
            workSchedule.Status = DAL.Enums.WorkTaskStatus.InProgress;
            workSchedule.UpdatedAt = DateTime.UtcNow;
            await _workScheduleRepo.UpdateAsync(workSchedule);
        }

        await _unitOfWork.SaveChangesAsync();
        return await GetWorkScheduleByIdAsync(workSchedule.Id);
    }

    public async Task<List<StaffAssignmentDetailDTO>> GetStaffAssignmentsAsync(
        int staffId,
        WorkScheduleFilterRequestDTO filter)
    {
        var queryBuilder = new QueryBuilder<StaffAssignment>()
            .WithTracking(false)
            .WithPredicate(sa => sa.StaffId == staffId)
            .WithInclude(sa => sa.WorkSchedule)
            .WithInclude(sa => sa.Staff);

        var query = _staffAssignmentRepo.Get(queryBuilder.Build());

        query = query
            .Include(sa => sa.WorkSchedule)
                .ThenInclude(ws => ws.TaskTemplate)
            .Include(sa => sa.WorkSchedule)
                .ThenInclude(ws => ws.PondAssignments)
                    .ThenInclude(pa => pa.Pond);

        var staffAssignments = await query.ToListAsync();
        var workScheduleIds = staffAssignments.Select(sa => sa.WorkScheduleId).Distinct().ToList();

        var staffCountsQuery = await _staffAssignmentRepo.Get(
            new QueryBuilder<StaffAssignment>()
                .WithTracking(false)
                .WithPredicate(sa => workScheduleIds.Contains(sa.WorkScheduleId))
                .Build())
            .ToListAsync();

        var staffCounts = staffCountsQuery
            .GroupBy(sa => sa.WorkScheduleId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Total = g.Count(),
                    Completed = g.Count(sa => sa.CompletedAt != null)
                }
            );

        var filteredAssignments = staffAssignments.Where(sa =>
        {
            var ws = sa.WorkSchedule;
            if (ws == null) return false;

            if (filter.Status.HasValue && ws.Status != filter.Status.Value)
                return false;

            if (filter.ScheduledDateFrom.HasValue && ws.ScheduledDate < filter.ScheduledDateFrom.Value)
                return false;

            if (filter.ScheduledDateTo.HasValue && ws.ScheduledDate > filter.ScheduledDateTo.Value)
                return false;

            return true;
        }).ToList();

        var result = filteredAssignments.Select(sa =>
        {
            var ws = sa.WorkSchedule;
            var taskTemplate = ws.TaskTemplate;

            var counts = staffCounts.TryGetValue(sa.WorkScheduleId, out var count)
                ? count
                : new { Total = 0, Completed = 0 };

            return new StaffAssignmentDetailDTO
            {
                WorkScheduleId = sa.WorkScheduleId,
                StaffId = sa.StaffId,
                CompletedAt = sa.CompletedAt,
                CompletionNotes = sa.CompletionNotes,
                IsCompleted = sa.CompletedAt != null,

                ScheduledDate = ws.ScheduledDate,
                StartTime = ws.StartTime,
                EndTime = ws.EndTime,
                Status = ws.Status,
                WorkScheduleNotes = ws.Notes,

                TaskTemplateId = taskTemplate?.Id ?? 0,
                TaskName = taskTemplate?.TaskName ?? "Unknown Task",
                TaskDescription = taskTemplate?.Description,
                DefaultDuration = taskTemplate?.DefaultDuration ?? 0,
                TaskNotes = taskTemplate?.NotesTask,

                PondNames = ws.PondAssignments
                    .Select(pa => pa.Pond?.PondName ?? "Unknown Pond")
                    .ToList(),

                TotalStaffAssigned = counts.Total,
                TotalStaffCompleted = counts.Completed
            };
        })
        .OrderByDescending(dto => dto.ScheduledDate)
        .ThenByDescending(dto => dto.StartTime)
        .ToList();

        return result;
    }
}
