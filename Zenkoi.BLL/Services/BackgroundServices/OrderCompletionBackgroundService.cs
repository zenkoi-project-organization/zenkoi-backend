using Microsoft.EntityFrameworkCore;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Zenkoi.BLL.Services.BackgroundServices
{
    public class OrderCompletionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan _autoCompleteAfter = TimeSpan.FromHours(24);

        public OrderCompletionBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessShippedOrders(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Error occurred while processing shipped orders
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessShippedOrders(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var cutoffTime = DateTime.UtcNow.Add(-_autoCompleteAfter);

                var shippedOrders = await unitOfWork.GetRepo<Order>().GetAllAsync(
                    new QueryBuilder<Order>()
                        .WithPredicate(o => o.Status == OrderStatus.Shipped && o.UpdatedAt <= cutoffTime)
                        .WithTracking(true)
                        .Build());

                if (!shippedOrders.Any())
                {
                    return;
                }

                var orderRepo = unitOfWork.GetRepo<Order>();
                var completedCount = 0;

                foreach (var order in shippedOrders)
                {
                    try
                    {
                        order.Status = OrderStatus.Delivered;
                        order.UpdatedAt = DateTime.UtcNow;

                        await orderRepo.UpdateAsync(order);
                        completedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Failed to auto-complete order
                    }
                }

                if (completedCount > 0)
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