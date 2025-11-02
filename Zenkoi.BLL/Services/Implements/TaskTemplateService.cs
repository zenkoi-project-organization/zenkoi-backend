using AutoMapper;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements;

public class TaskTemplateService : ITaskTemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepoBase<TaskTemplate> _taskTemplateRepo;

    public TaskTemplateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _taskTemplateRepo = _unitOfWork.GetRepo<TaskTemplate>();
    }

    public async Task<PaginatedList<TaskTemplateResponseDTO>> GetAllTaskTemplatesAsync(
        TaskTemplateFilterRequestDTO filter,
        int pageIndex = 1,
        int pageSize = 10)
    {
        var queryBuilder = new QueryBuilder<TaskTemplate>()
            .WithTracking(false);

        if (!string.IsNullOrEmpty(filter.Search))
        {
            queryBuilder.WithPredicate(t =>
                t.TaskName.Contains(filter.Search) ||
                (t.Description != null && t.Description.Contains(filter.Search)));
        }

        if (filter.IsRecurring.HasValue)
        {
            queryBuilder.WithPredicate(t => t.IsRecurring == filter.IsRecurring.Value);
        }

        if (filter.IsDeleted.HasValue)
        {
            queryBuilder.WithPredicate(t => t.IsDeleted == filter.IsDeleted.Value);
        }
        else
        {
            queryBuilder.WithPredicate(t => t.IsDeleted == false);
        }

        queryBuilder.WithOrderBy(q => q.OrderByDescending(t => t.CreatedAt));

        var query = _taskTemplateRepo.Get(queryBuilder.Build());
        var paginatedEntities = await PaginatedList<TaskTemplate>.CreateAsync(query, pageIndex, pageSize);
        var resultDto = _mapper.Map<List<TaskTemplateResponseDTO>>(paginatedEntities);

        return new PaginatedList<TaskTemplateResponseDTO>(
            resultDto,
            paginatedEntities.TotalItems,
            pageIndex,
            pageSize);
    }

    public async Task<TaskTemplateResponseDTO> GetTaskTemplateByIdAsync(int id)
    {
        var taskTemplate = await _taskTemplateRepo.GetSingleAsync(new QueryBuilder<TaskTemplate>()
            .WithPredicate(t => t.Id == id)
            .WithTracking(false)
            .Build());

        if (taskTemplate == null)
            throw new ArgumentException("Task template not found");

        return _mapper.Map<TaskTemplateResponseDTO>(taskTemplate);
    }

    public async Task<TaskTemplateResponseDTO> CreateTaskTemplateAsync(TaskTemplateRequestDTO dto)
    {
        var taskTemplate = _mapper.Map<TaskTemplate>(dto);
        taskTemplate.CreatedAt = DateTime.UtcNow;
        taskTemplate.IsDeleted = false;

        await _taskTemplateRepo.CreateAsync(taskTemplate);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TaskTemplateResponseDTO>(taskTemplate);
    }

    public async Task<TaskTemplateResponseDTO> UpdateTaskTemplateAsync(int id, TaskTemplateRequestDTO dto)
    {
        var taskTemplate = await _taskTemplateRepo.GetByIdAsync(id);

        if (taskTemplate == null)
            throw new ArgumentException("Task template not found");

        _mapper.Map(dto, taskTemplate);
        taskTemplate.UpdatedAt = DateTime.UtcNow;

        await _taskTemplateRepo.UpdateAsync(taskTemplate);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TaskTemplateResponseDTO>(taskTemplate);
    }

    public async Task<bool> DeleteTaskTemplateAsync(int id)
    {
        var taskTemplate = await _taskTemplateRepo.GetByIdAsync(id);

        if (taskTemplate == null)
            return false;

        taskTemplate.IsDeleted = true;
        taskTemplate.UpdatedAt = DateTime.UtcNow;

        await _taskTemplateRepo.UpdateAsync(taskTemplate);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RestoreTaskTemplateAsync(int id)
    {
        var taskTemplate = await _taskTemplateRepo.GetByIdAsync(id);

        if (taskTemplate == null)
            return false;

        taskTemplate.IsDeleted = false;
        taskTemplate.UpdatedAt = DateTime.UtcNow;

        await _taskTemplateRepo.UpdateAsync(taskTemplate);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
