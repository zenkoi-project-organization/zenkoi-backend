using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class SalesDashboardService : ISalesDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepoBase<Order> _orderRepo;
        private readonly IRepoBase<OrderDetail> _orderDetailRepo;
        private readonly IRepoBase<Customer> _customerRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<PacketFish> _packetFishRepo;

        public SalesDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orderRepo = _unitOfWork.GetRepo<Order>();
            _orderDetailRepo = _unitOfWork.GetRepo<OrderDetail>();
            _customerRepo = _unitOfWork.GetRepo<Customer>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
        }

        public async Task<SalesStatisticsDTO> GetStatisticsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            // Monthly Revenue
            var currentMonthRevenue = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CreatedAt >= startOfMonth && 
                                    o.Status != OrderStatus.Cancelled)
                .Build());

            var lastMonthRevenue = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CreatedAt >= startOfLastMonth && 
                                    o.CreatedAt <= endOfLastMonth &&
                                    o.Status != OrderStatus.Cancelled)
                .Build());

            var currentMonthRevenueList = currentMonthRevenue.ToList();
            var lastMonthRevenueList = lastMonthRevenue.ToList();

            var currentRevenue = currentMonthRevenueList.Sum(o => o.TotalAmount);
            var lastRevenue = lastMonthRevenueList.Sum(o => o.TotalAmount);
            var revenueChangePercent = 0.0;
            if (lastRevenue > 0)
            {
                revenueChangePercent = (double)((currentRevenue - lastRevenue) / lastRevenue * 100);
            }
            else if (currentRevenue > 0)
            {
             
                revenueChangePercent = 100.0;
            }

            // Total Orders
            var currentMonthOrders = currentMonthRevenueList.Count;
            var lastMonthOrders = lastMonthRevenueList.Count;
            var ordersChangePercent = 0.0;
            if (lastMonthOrders > 0)
            {
                ordersChangePercent = (double)((currentMonthOrders - lastMonthOrders) / (double)lastMonthOrders * 100);
            }
            else if (currentMonthOrders > 0)
            {
               
                ordersChangePercent = 100.0;
            }

            // Customer Count
            var allCustomers = await _customerRepo.GetAllAsync(new QueryBuilder<Customer>().Build());
            var currentCustomers = allCustomers.Count(c => c.IsActive);
            
            var newCustomersThisMonth = allCustomers.Count(c => 
                c.CreatedAt >= startOfMonth && c.IsActive);
            var newCustomerPercent = currentCustomers > 0 
                ? (double)(newCustomersThisMonth / (double)currentCustomers * 100) 
                : 0;

            // Fish In Stock
            var allKoiFish = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                .WithPredicate(k => k.SaleStatus == SaleStatus.Available)
                .Build());
            var fishInStock = allKoiFish.Count();
            

            var avgPrice = allKoiFish.Any() ? allKoiFish.Average(k => (double)(k.SellingPrice ?? 0)) : 0;
            var lowStockCount = allKoiFish.Count(k => (k.SellingPrice ?? 0) < (decimal)avgPrice * 0.5m || 
                                                      allKoiFish.Count() < 10);

            return new SalesStatisticsDTO
            {
                MonthlyRevenue = new MonthlyRevenueDTO
                {
                    Current = currentRevenue,
                    ChangePercent = Math.Round(revenueChangePercent, 1)
                },
                TotalOrders = new TotalOrdersDTO
                {
                    Current = currentMonthOrders,
                    ChangePercent = Math.Round(ordersChangePercent, 1)
                },
                CustomerCount = new CustomerCountDTO
                {
                    Current = currentCustomers,
                    NewCustomerPercent = Math.Round(newCustomerPercent, 1)
                },
                FishInStock = new FishInStockDTO
                {
                    Current = fishInStock,
                    LowStockCount = lowStockCount
                }
            };
        }

        public async Task<SalesQuickInfoDTO> GetQuickInfoAsync()
        {
            var pendingOrders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Status == OrderStatus.Pending ||
                                    o.Status == OrderStatus.Processing)
                .Build());

            var completedOrders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Status == OrderStatus.Delivered)
                .Build());

            var orderDetails = await _orderDetailRepo.GetAllAsync(new QueryBuilder<OrderDetail>()
                .WithInclude(od => od.KoiFish)
                .WithInclude(od => od.KoiFish.Variety)
                .WithInclude(od => od.PacketFish)
                .Build());

            var bestSellingVarieties = orderDetails
                .Where(od => od.KoiFish != null || od.PacketFish != null)
                .Select(od => od.KoiFish != null ? od.KoiFish.VarietyId : 
                             (od.PacketFish != null ? od.PacketFish.Id : 0))
                .Distinct()
                .Count();

            return new SalesQuickInfoDTO
            {
                PendingOrders = pendingOrders.Count(),
                CompletedOrders = completedOrders.Count(),
                BestSellingItemsCount = bestSellingVarieties,
                Alerts = new List<string> { "Không có thông tin quan trọng cần xử lý." }
            };
        }

        public async Task<List<BestSellerDTO>> GetBestSellersAsync(int top = 5)
        {
            var validOrders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.Status != OrderStatus.Cancelled)
                .Build());

            var validOrderIds = validOrders.Select(o => o.Id).ToList();

            var orderDetails = await _orderDetailRepo.GetAllAsync(new QueryBuilder<OrderDetail>()
                .WithInclude(od => od.KoiFish)
                .WithInclude(od => od.KoiFish.Variety)
                .WithInclude(od => od.PacketFish)
                .WithPredicate(od => validOrderIds.Contains(od.OrderId))
                .Build());

            var bestSellers = new List<BestSellerDTO>();

            var koiFishGroups = orderDetails
                .Where(od => od.KoiFishId.HasValue && od.KoiFish != null)
                .GroupBy(od => new { 
                    Id = od.KoiFish.VarietyId, 
                    Name = od.KoiFish.Variety != null ? od.KoiFish.Variety.VarietyName : "Unknown" 
                })
                .Select(g => new BestSellerDTO
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    SoldCount = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.TotalPrice)
                })
                .ToList();

            var packetFishGroups = orderDetails
                .Where(od => od.PacketFishId.HasValue && od.PacketFish != null)
                .GroupBy(od => new { 
                    Id = od.PacketFish.Id, 
                    Name = od.PacketFish.Name 
                })
                .Select(g => new BestSellerDTO
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    SoldCount = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.TotalPrice)
                })
                .ToList();

            bestSellers.AddRange(koiFishGroups);
            bestSellers.AddRange(packetFishGroups);

            return bestSellers
                .OrderByDescending(bs => bs.SoldCount)
                .Take(top)
                .ToList();
        }

        public async Task<SalesAnalysisDTO> GetSalesAnalysisAsync(SalesAnalysisRange range = SalesAnalysisRange.Last30Days)
        {
            var now = DateTime.UtcNow;
            DateTime startDate;
            int periodCount;
            Func<DateTime, DateTime> periodStartFunc;
            Func<DateTime, string> labelFunc;

            if (range == SalesAnalysisRange.Last12Months)
            {
                startDate = now.AddMonths(-12);
                periodCount = 12;
                periodStartFunc = d => new DateTime(d.Year, d.Month, 1);
                labelFunc = d => $"Tháng {d.Month}/{d.Year}";
            }
            else 
            {
                startDate = now.AddDays(-30);
                periodCount = 4; 
                periodStartFunc = d => d.AddDays(-(int)d.DayOfWeek).Date;
                labelFunc = d => $"Tuần {((d - startDate).Days / 7) + 1}";
            }

            var orders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CreatedAt >= startDate &&
                                    o.Status != OrderStatus.Cancelled)
                .Build());

            var ordersList = orders.ToList();
            var labels = new List<string>();
            var revenueData = new List<decimal>();
            var ordersData = new List<int>();

            for (int i = 0; i < periodCount; i++)
            {
                DateTime periodStart;
                DateTime periodEnd;

                if (range == SalesAnalysisRange.Last12Months)
                {
                    periodStart = startDate.AddMonths(i);
                    periodEnd = periodStart.AddMonths(1);
                    labels.Add(labelFunc(periodStart));
                }
                else
                {
                    periodStart = startDate.AddDays(i * 7);
                    periodEnd = periodStart.AddDays(7);
                    labels.Add(labelFunc(periodStart));
                }

                var periodOrders = ordersList.Where(o => o.CreatedAt >= periodStart && o.CreatedAt < periodEnd);
                revenueData.Add(periodOrders.Sum(o => o.TotalAmount));
                ordersData.Add(periodOrders.Count());
            }

            return new SalesAnalysisDTO
            {
                Labels = labels,
                RevenueData = revenueData,
                OrdersData = ordersData
            };
        }   
    }
}

