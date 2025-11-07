using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zenkoi.BLL.DTOs.TaskTemplateDTOs;
using Zenkoi.BLL.DTOs.WeeklyScheduleTemplateDTOs;
using Zenkoi.BLL.DTOs.WorkScheduleDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements;

public class WeeklyScheduleTemplateService : IWeeklyScheduleTemplateService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepoBase<WeeklyScheduleTemplate> _templateRepo;
    private readonly IRepoBase<WeeklyScheduleTemplateItem> _templateItemRepo;
    private readonly IRepoBase<TaskTemplate> _taskTemplateRepo;
    private readonly IRepoBase<WorkSchedule> _workScheduleRepo;

    public WeeklyScheduleTemplateService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _templateRepo = _unitOfWork.GetRepo<WeeklyScheduleTemplate>();
        _templateItemRepo = _unitOfWork.GetRepo<WeeklyScheduleTemplateItem>();
        _taskTemplateRepo = _unitOfWork.GetRepo<TaskTemplate>();
        _workScheduleRepo = _unitOfWork.GetRepo<WorkSchedule>();
    }

    public async Task<WeeklyScheduleTemplateResponseDTO> CreateTemplateAsync(WeeklyScheduleTemplateRequestDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            foreach (var item in dto.TemplateItems)
            {
                var taskTemplate = await _taskTemplateRepo.GetByIdAsync(item.TaskTemplateId);
                if (taskTemplate == null)
                {
                    throw new ArgumentException($"TaskTemplate with ID {item.TaskTemplateId} not found");
                }

                if (item.EndTime <= item.StartTime)
                {
                    throw new ArgumentException("EndTime must be greater than StartTime");
                }
            }

            var template = _mapper.Map<WeeklyScheduleTemplate>(dto);
            template.CreatedAt = DateTime.UtcNow;

            await _templateRepo.CreateAsync(template);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetTemplateByIdAsync(template.Id);
        }
        catch
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<WeeklyScheduleTemplateResponseDTO> GetTemplateByIdAsync(int id)
    {
        var template = await _templateRepo.GetSingleAsync(new QueryOptions<WeeklyScheduleTemplate>
        {
            Predicate = t => t.Id == id && t.IsActive,
            IncludeProperties = new List<System.Linq.Expressions.Expression<Func<WeeklyScheduleTemplate, object>>>
            {
                t => t.TemplateItems
            }
        });

        if (template == null)
        {
            throw new KeyNotFoundException("Weekly schedule template not found");
        }

        var dto = _mapper.Map<WeeklyScheduleTemplateResponseDTO>(template);

        foreach (var item in dto.TemplateItems)
        {
            var taskTemplate = await _taskTemplateRepo.GetByIdAsync(item.TaskTemplateId);
            item.TaskTemplate = _mapper.Map<TaskTemplateResponseDTO>(taskTemplate);
        }

        return dto;
    }

    public async Task<List<WeeklyScheduleTemplateResponseDTO>> GetAllTemplatesAsync()
    {
        var templates = await _templateRepo.GetAllAsync(new QueryOptions<WeeklyScheduleTemplate>
        {
            Predicate = t => t.IsActive,
            IncludeProperties = new List<System.Linq.Expressions.Expression<Func<WeeklyScheduleTemplate, object>>>
            {
                t => t.TemplateItems
            }
        });

        var dtos = _mapper.Map<List<WeeklyScheduleTemplateResponseDTO>>(templates);

        foreach (var dto in dtos)
        {
            foreach (var item in dto.TemplateItems)
            {
                var taskTemplate = await _taskTemplateRepo.GetByIdAsync(item.TaskTemplateId);
                item.TaskTemplate = _mapper.Map<TaskTemplateResponseDTO>(taskTemplate);
            }
        }

        return dtos;
    }

    public async Task<WeeklyScheduleTemplateResponseDTO> UpdateTemplateAsync(int id, WeeklyScheduleTemplateRequestDTO dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var template = await _templateRepo.GetByIdAsync(id);
            if (template == null || !template.IsActive)
            {
                throw new KeyNotFoundException("Weekly schedule template not found");
            }

            foreach (var item in dto.TemplateItems)
            {
                var taskTemplate = await _taskTemplateRepo.GetByIdAsync(item.TaskTemplateId);
                if (taskTemplate == null)
                {
                    throw new ArgumentException($"TaskTemplate with ID {item.TaskTemplateId} not found");
                }

                if (item.EndTime <= item.StartTime)
                {
                    throw new ArgumentException("EndTime must be greater than StartTime");
                }
            }

            var existingItems = await _templateItemRepo.GetAllAsync(new QueryOptions<WeeklyScheduleTemplateItem>
            {
                Predicate = i => i.WeeklyScheduleTemplateId == id
            });

            foreach (var existingItem in existingItems)
            {
                await _templateItemRepo.DeleteAsync(existingItem);
            }

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.UpdatedAt = DateTime.UtcNow;

            await _templateRepo.UpdateAsync(template);

            foreach (var itemDto in dto.TemplateItems)
            {
                var item = _mapper.Map<WeeklyScheduleTemplateItem>(itemDto);
                item.WeeklyScheduleTemplateId = id;
                await _templateItemRepo.CreateAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return await GetTemplateByIdAsync(id);
        }
        catch
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteTemplateAsync(int id)
    {
        var template = await _templateRepo.GetByIdAsync(id);
        if (template == null)
        {
            throw new KeyNotFoundException("Weekly schedule template not found");
        }

        template.IsActive = false;
        template.UpdatedAt = DateTime.UtcNow;

        await _templateRepo.UpdateAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<List<WorkScheduleResponseDTO>> GenerateWorkSchedulesFromTemplateAsync(GenerateScheduleRequestDTO request)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var template = await _templateRepo.GetSingleAsync(new QueryOptions<WeeklyScheduleTemplate>
            {
                Predicate = t => t.Id == request.WeeklyScheduleTemplateId && t.IsActive,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<WeeklyScheduleTemplate, object>>>
                {
                    t => t.TemplateItems
                }
            });

            if (template == null)
            {
                throw new KeyNotFoundException("Weekly schedule template not found");
            }

            var startDate = request.StartDate;
            var startDayOfWeek = startDate.DayOfWeek;

            var generatedSchedules = new List<WorkSchedule>();

            var scheduleDates = new HashSet<DateOnly>();
            foreach (var templateItem in template.TemplateItems)
            {
                int daysToAdd = ((int)templateItem.DayOfWeek - (int)startDayOfWeek + 7) % 7;
                var scheduleDate = startDate.AddDays(daysToAdd);

                if (!scheduleDates.Add(scheduleDate))
                {
                    continue;
                }

                var existingSchedule = await _workScheduleRepo.GetSingleAsync(new QueryOptions<WorkSchedule>
                {
                    Predicate = ws => ws.ScheduledDate == scheduleDate
                });

                if (existingSchedule != null)
                {
                    throw new InvalidOperationException($"Duplicate schedule detected: A schedule already exists for date {scheduleDate:yyyy-MM-dd}");
                }
            }

            foreach (var templateItem in template.TemplateItems)
            {
                int daysToAdd = ((int)templateItem.DayOfWeek - (int)startDayOfWeek + 7) % 7;
                var scheduleDate = startDate.AddDays(daysToAdd);

                var workSchedule = new WorkSchedule
                {
                    TaskTemplateId = templateItem.TaskTemplateId,
                    ScheduledDate = scheduleDate,
                    StartTime = templateItem.StartTime,
                    EndTime = templateItem.EndTime,
                    Status = WorkTaskStatus.Pending,
                    CreatedBy = 1,
                    CreatedAt = DateTime.UtcNow
                };

                await _workScheduleRepo.CreateAsync(workSchedule);
                generatedSchedules.Add(workSchedule);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            var result = new List<WorkScheduleResponseDTO>();
            foreach (var schedule in generatedSchedules)
            {
                var scheduleWithRelations = await _workScheduleRepo.GetSingleAsync(new QueryOptions<WorkSchedule>
                {
                    Predicate = ws => ws.Id == schedule.Id,
                    IncludeProperties = new List<System.Linq.Expressions.Expression<Func<WorkSchedule, object>>>
                    {
                        ws => ws.TaskTemplate
                    }
                });
                result.Add(_mapper.Map<WorkScheduleResponseDTO>(scheduleWithRelations));
            }

            return result;
        }
        catch
        {
            await _unitOfWork.RollBackAsync();
            throw;
        }
    }
}
