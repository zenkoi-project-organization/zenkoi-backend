using Microsoft.Extensions.Hosting;
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
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _orderTimeout = TimeSpan.FromMinutes(30);

        public PendingOrderCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredPendingOrdersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
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
                    return;
                }

                var cleanedCount = 0;

                foreach (var order in expiredOrders)
                {
                    try
                    {
                        // Cancel all pending payment transactions for this order
                        await CancelPendingPaymentTransactionsAsync(unitOfWork, order.Id);

                        // Cancel order and release inventory
                        await orderService.CancelOrderAndReleaseInventoryAsync(order.Id);
                        cleanedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Failed to cancel expired order
                    }
                }
            }
            catch (Exception ex)
            {
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
                    return;
                }

                foreach (var transaction in pendingTransactions)
                {
                    transaction.Status = PaymentTransactionStatus.Cancelled;
                    transaction.UpdatedAt = DateTime.UtcNow;
                    await paymentTransactionRepo.UpdateAsync(transaction);
                }

                await unitOfWork.SaveChangesAsync();
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
