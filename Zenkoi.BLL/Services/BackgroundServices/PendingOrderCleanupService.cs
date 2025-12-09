using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Enums;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.BackgroundServices
{
    public class PendingOrderCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PendingOrderCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _orderTimeout = TimeSpan.FromMinutes(30);

        public PendingOrderCleanupService(
            IServiceProvider serviceProvider,
            ILogger<PendingOrderCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PendingOrderCleanupService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredPendingOrdersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up expired pending orders");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("PendingOrderCleanupService stopped");
        }

        private async Task CleanupExpiredPendingOrdersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var cutoffTime = DateTime.UtcNow.Subtract(_orderTimeout);

                var expiredOrders = await unitOfWork.GetRepo<Order>().GetAllAsync(
                    new QueryBuilder<Order>()
                        .WithPredicate(o =>
                            o.Status == OrderStatus.Pending &&
                            o.CreatedAt < cutoffTime)
                        .WithTracking(false)
                        .Build());

                if (!expiredOrders.Any())
                {
                    _logger.LogInformation("No expired pending orders to clean up at {Time}", DateTime.UtcNow);
                    return;
                }

                _logger.LogInformation("Found {Count} expired pending orders to clean up", expiredOrders.Count());

                var cleanedCount = 0;

                foreach (var order in expiredOrders)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Auto-canceling expired Order #{OrderId} (created at {CreatedAt})",
                            order.Id,
                            order.CreatedAt);

                        // Cancel all pending payment transactions for this order
                        await CancelPendingPaymentTransactionsAsync(unitOfWork, order.Id);

                        // Cancel order and release inventory
                        await orderService.CancelOrderAndReleaseInventoryAsync(order.Id);
                        cleanedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to cancel expired Order #{OrderId}", order.Id);
                    }
                }

                if (cleanedCount > 0)
                {
                    _logger.LogInformation("Successfully cleaned up {Count} expired orders", cleanedCount);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CleanupExpiredPendingOrdersAsync");
                throw;
            }
        }

        private async Task CancelPendingPaymentTransactionsAsync(IUnitOfWork unitOfWork, int orderId)
        {
            try
            {
                var paymentTransactionRepo = unitOfWork.GetRepo<PaymentTransaction>();

                var pendingTransactions = await paymentTransactionRepo.GetAllAsync(
                    new QueryBuilder<PaymentTransaction>()
                        .WithPredicate(pt =>
                            pt.ActualOrderId == orderId &&
                            pt.Status == PaymentTransactionStatus.Pending)
                        .WithTracking(true)
                        .Build());

                if (!pendingTransactions.Any())
                {
                    _logger.LogInformation("No pending payment transactions found for Order #{OrderId}", orderId);
                    return;
                }

                _logger.LogInformation("Found {Count} pending payment transactions for Order #{OrderId}",
                    pendingTransactions.Count(), orderId);

                foreach (var transaction in pendingTransactions)
                {
                    transaction.Status = PaymentTransactionStatus.Cancelled;
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await paymentTransactionRepo.UpdateAsync(transaction);

                    _logger.LogInformation(
                        "Cancelled PaymentTransaction #{TransactionId} for Order #{OrderId}",
                        transaction.Id,
                        orderId);
                }

                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling pending payment transactions for Order #{OrderId}", orderId);
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PendingOrderCleanupService is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
