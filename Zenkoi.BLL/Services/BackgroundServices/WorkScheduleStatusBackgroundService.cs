using Microsoft.EntityFrameworkCore;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Zenkoi.BLL.Services.BackgroundServices
{
    public class WorkScheduleStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WorkScheduleStatusBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public WorkScheduleStatusBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<WorkScheduleStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkScheduleStatusBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessActiveWorkSchedules(stoppingToken);
                    await ProcessOverdueWorkSchedules(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing work schedules");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("WorkScheduleStatusBackgroundService stopped");
        }

        private async Task ProcessActiveWorkSchedules(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var now = DateTime.Now;
                var currentDate = DateOnly.FromDateTime(now);
                var currentTime = TimeOnly.FromDateTime(now);

                var activeSchedules = await unitOfWork.GetRepo<WorkSchedule>().GetAllAsync(
                    new QueryBuilder<WorkSchedule>()
                        .WithPredicate(ws =>
                            ws.Status == WorkTaskStatus.Pending &&
                            ws.ScheduledDate == currentDate &&
                            ws.StartTime <= currentTime &&
                            ws.EndTime >= currentTime)
                        .WithTracking(true)
                        .Build());

                if (!activeSchedules.Any())
                {
                    _logger.LogInformation("No active work schedules to process at {Time}", DateTime.UtcNow);
                    return;
                }

                _logger.LogInformation("Found {Count} active work schedules to process", activeSchedules.Count());

                var workScheduleRepo = unitOfWork.GetRepo<WorkSchedule>();
                var updatedCount = 0;

                foreach (var schedule in activeSchedules)
                {
                    try
                    {
                        schedule.Status = WorkTaskStatus.InProgress;
                        schedule.UpdatedAt = DateTime.UtcNow;

                        await workScheduleRepo.UpdateAsync(schedule);
                        updatedCount++;

                        _logger.LogInformation(
                            "Auto-marking WorkSchedule #{ScheduleId} as InProgress - Currently within working hours",
                            schedule.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to update WorkSchedule #{ScheduleId} to InProgress", schedule.Id);
                    }
                }

                if (updatedCount > 0)
                {
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated {Count} work schedules to InProgress", updatedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessActiveWorkSchedules");
                throw;
            }
        }

        private async Task ProcessOverdueWorkSchedules(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var now = DateTime.Now;
                var currentDate = DateOnly.FromDateTime(now);
                var currentTime = TimeOnly.FromDateTime(now);
            
                var overdueSchedules = await unitOfWork.GetRepo<WorkSchedule>().GetAllAsync(
                    new QueryBuilder<WorkSchedule>()
                        .WithPredicate(ws =>
                            ws.Status != WorkTaskStatus.Completed &&
                            ws.Status != WorkTaskStatus.Cancelled &&
                            ws.Status != WorkTaskStatus.Incomplete &&
                            (ws.ScheduledDate < currentDate ||
                             (ws.ScheduledDate == currentDate && ws.EndTime < currentTime)))
                        .WithTracking(true)
                        .WithInclude(ws => ws.StaffAssignments)
                        .Build());

                if (!overdueSchedules.Any())
                {
                    _logger.LogInformation("No overdue work schedules to process at {Time}", DateTime.UtcNow);
                    return;
                }

                _logger.LogInformation("Found {Count} overdue work schedules to process", overdueSchedules.Count());

                var workScheduleRepo = unitOfWork.GetRepo<WorkSchedule>();
                var updatedCount = 0;

                foreach (var schedule in overdueSchedules)
                {
                    try
                    {
                        WorkTaskStatus newStatus;

                        if (schedule.StaffAssignments != null && schedule.StaffAssignments.Any())
                        {
                            bool allStaffCompleted = schedule.StaffAssignments.All(sa => sa.CompletedAt != null);

                            if (allStaffCompleted)
                            {
                                newStatus = WorkTaskStatus.Completed;
                                _logger.LogInformation(
                                    "Auto-marking WorkSchedule #{ScheduleId} as Completed - All staff completed",
                                    schedule.Id);
                            }
                            else
                            {
                                newStatus = WorkTaskStatus.Incomplete;
                                var completedCount = schedule.StaffAssignments.Count(sa => sa.CompletedAt != null);
                                _logger.LogInformation(
                                    "Auto-marking WorkSchedule #{ScheduleId} as Incomplete - {Completed}/{Total} staff completed",
                                    schedule.Id,
                                    completedCount,
                                    schedule.StaffAssignments.Count);
                            }
                        }
                        else
                        {
                            newStatus = WorkTaskStatus.Incomplete;
                            _logger.LogInformation(
                                "Auto-marking WorkSchedule #{ScheduleId} as Incomplete - No staff assigned",
                                schedule.Id);
                        }

                        schedule.Status = newStatus;
                        schedule.UpdatedAt = DateTime.UtcNow;

                        await workScheduleRepo.UpdateAsync(schedule);
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to update WorkSchedule #{ScheduleId}", schedule.Id);
                    }
                }

                if (updatedCount > 0)
                {
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully updated {Count} work schedules", updatedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessOverdueWorkSchedules");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkScheduleStatusBackgroundService is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
