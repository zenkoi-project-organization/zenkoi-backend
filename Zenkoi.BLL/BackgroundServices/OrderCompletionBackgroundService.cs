using Microsoft.EntityFrameworkCore;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Zenkoi.BLL.BackgroundServices
{
    public class OrderCompletionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCompletionBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); 
        private readonly TimeSpan _autoCompleteAfter = TimeSpan.FromHours(24);

        public OrderCompletionBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OrderCompletionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCompletionBackgroundService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessShippedOrders(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing shipped orders");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("OrderCompletionBackgroundService stopped");
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
                    _logger.LogInformation("No shipped orders to auto-complete at {Time}", DateTime.UtcNow);
                    return;
                }

                _logger.LogInformation("Found {Count} shipped orders to auto-complete", shippedOrders.Count());

                var orderRepo = unitOfWork.GetRepo<Order>();
                var completedCount = 0;

                foreach (var order in shippedOrders)
                {
                    try
                    {
                        var timeShipped = DateTime.UtcNow - (order.UpdatedAt ?? order.CreatedAt);

                        _logger.LogInformation(
                            "Auto-completing Order #{OrderId} (OrderNumber: {OrderNumber}) - Shipped for {Hours:F1} hours",
                            order.Id,
                            order.OrderNumber,
                            timeShipped.TotalHours);

                        order.Status = OrderStatus.Completed;
                        order.UpdatedAt = DateTime.UtcNow;

                        await orderRepo.UpdateAsync(order);
                        completedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to auto-complete Order #{OrderId}", order.Id);
                    }
                }

                if (completedCount > 0)
                {
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully auto-completed {Count} orders", completedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessShippedOrders");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrderCompletionBackgroundService is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}