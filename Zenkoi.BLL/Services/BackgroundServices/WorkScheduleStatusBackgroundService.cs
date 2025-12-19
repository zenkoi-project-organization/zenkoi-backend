using Microsoft.EntityFrameworkCore;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Zenkoi.BLL.Services.BackgroundServices
{
    public class WorkScheduleStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

        public WorkScheduleStatusBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessActiveWorkSchedules(stoppingToken);
                    await ProcessOverdueWorkSchedules(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Error occurred while processing work schedules
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
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
                    return;
                }

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
                    }
                    catch (Exception ex)
                    {
                        // Failed to update WorkSchedule to InProgress
                    }
                }

                if (updatedCount > 0)
                {
                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
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
                    return;
                }

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
                            }
                            else
                            {
                                newStatus = WorkTaskStatus.Incomplete;
                            }
                        }
                        else
                        {
                            newStatus = WorkTaskStatus.Incomplete;
                        }

                        schedule.Status = newStatus;
                        schedule.UpdatedAt = DateTime.UtcNow;

                        await workScheduleRepo.UpdateAsync(schedule);
                        updatedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Failed to update WorkSchedule
                    }
                }

                if (updatedCount > 0)
                {
                    await unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
