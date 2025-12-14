using Zenkoi.BLL.DTOs.DashboardDTOs.FarmDashboardDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class FarmDashboardService : IFarmDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;
        private readonly IRepoBase<Pond> _pondRepo;
        private readonly IRepoBase<Order> _orderRepo;
        private readonly IRepoBase<BreedingProcess> _breedingProcessRepo;

        public FarmDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _orderRepo = _unitOfWork.GetRepo<Order>();
            _breedingProcessRepo = _unitOfWork.GetRepo<BreedingProcess>();
        }

        public async Task<FarmStatisticsDTO> GetStatisticsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            // Total Koi
            var allKoi = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>().Build());
            var allKoiList = allKoi.ToList();
            var currentKoiCount = allKoiList.Count;

            var lastMonthKoi = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                .WithPredicate(k => k.CreatedAt < startOfMonth)
                .Build());
            var lastMonthKoiList = lastMonthKoi.ToList();
            var lastMonthKoiCount = lastMonthKoiList.Count;
            var koiChangePercent = 0.0;
            if (lastMonthKoiCount > 0)
            {
                koiChangePercent = (double)((currentKoiCount - lastMonthKoiCount) / (double)lastMonthKoiCount * 100);
            }
            else if (currentKoiCount > 0)
            {             
                koiChangePercent = 100.0;
            }

            var allUsers = await _userRepo.GetAllAsync(new QueryBuilder<ApplicationUser>()
                .WithPredicate(u => u.Role == Role.FarmStaff || u.Role == Role.Manager || u.Role == Role.SaleStaff)
                .Build());
            var allUsersList = allUsers.ToList();
            var currentActiveAccounts = allUsersList.Count(u => !u.IsDeleted && !u.IsBlocked);

            var accountsChangePercent = 0.0;

            var allPonds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>().Build());
            var allPondsList = allPonds.ToList();
            var currentPondsInUse = allPondsList.Count(p => p.PondStatus == PondStatus.Active);

            var lastMonthPonds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>()
                .WithPredicate(p => p.CreatedAt < startOfMonth)
                .Build());
            var lastMonthPondsList = lastMonthPonds.ToList();
            var lastMonthPondsInUse = lastMonthPondsList.Count(p => p.PondStatus == PondStatus.Active);
            var pondsChangePercent = 0.0;
            if (lastMonthPondsInUse > 0)
            {
                pondsChangePercent = (double)((currentPondsInUse - lastMonthPondsInUse) / (double)lastMonthPondsInUse * 100);
            }
            else if (currentPondsInUse > 0)
            {             
                pondsChangePercent = 100.0;
            }

            var currentMonthOrders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CreatedAt >= startOfMonth &&
                                    o.Status != OrderStatus.Cancelled &&
                                    o.Status != OrderStatus.Refund &&
                                    o.Status != OrderStatus.Rejected &&
                                    o.Status != OrderStatus.UnShiping)
                .Build());

            var lastMonthOrders = await _orderRepo.GetAllAsync(new QueryBuilder<Order>()
                .WithPredicate(o => o.CreatedAt >= startOfLastMonth &&
                                    o.CreatedAt <= endOfLastMonth &&
                                    o.Status != OrderStatus.Cancelled &&
                                    o.Status != OrderStatus.Refund &&
                                    o.Status != OrderStatus.Rejected &&
                                    o.Status != OrderStatus.UnShiping)
                .Build());

            var currentMonthOrdersList = currentMonthOrders.ToList();
            var lastMonthOrdersList = lastMonthOrders.ToList();
            
            var currentRevenue = currentMonthOrdersList.Sum(o => o.TotalAmount);
            var lastRevenue = lastMonthOrdersList.Sum(o => o.TotalAmount);
            var revenueChangePercent = 0.0;
            if (lastRevenue > 0)
            {
                revenueChangePercent = (double)((currentRevenue - lastRevenue) / lastRevenue * 100);
            }
            else if (currentRevenue > 0)
            {
                revenueChangePercent = 100.0;
            }

            return new FarmStatisticsDTO
            {
                TotalKoi = new TotalKoiDTO
                {
                    Current = currentKoiCount,
                    ChangePercent = Math.Round(koiChangePercent, 1)
                },
                ActiveAccounts = new ActiveAccountsDTO
                {
                    Current = currentActiveAccounts,
                    ChangePercent = Math.Round(accountsChangePercent, 1)
                },
                PondsInUse = new PondsInUseDTO
                {
                    Current = currentPondsInUse,
                    ChangePercent = Math.Round(pondsChangePercent, 1)
                },
                FarmMonthlyRevenue = new FarmMonthlyRevenueDTO
                {
                    Current = currentRevenue,
                    ChangePercent = Math.Round(revenueChangePercent, 1)
                }
            };
        }

        public async Task<List<ActivityFeedDTO>> GetActivityFeedAsync(int limit = 5)
        {
            var activities = new List<ActivityFeedDTO>();
            var now = DateTime.UtcNow;

            var recentKoi = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>()
                .WithInclude(k => k.Pond)
                .WithInclude(k => k.Variety)
                .WithOrderBy(k => k.OrderByDescending(x => x.CreatedAt))
                .Build());

            var recentKoiList = recentKoi.Take(3).ToList();
            foreach (var koi in recentKoiList)
            {
                activities.Add(new ActivityFeedDTO
                {
                    Type = "NEW_KOI",
                    Message = $"Thêm cá Koi {koi.Variety?.VarietyName ?? "Unknown"} mới vào hồ {koi.Pond?.PondName ?? "Unknown"}",
                    Timestamp = koi.CreatedAt
                });
            }

            var recentBreeding = await _breedingProcessRepo.GetAllAsync(new QueryBuilder<BreedingProcess>()
                .WithInclude(bp => bp.MaleKoi)
                .WithInclude(bp => bp.FemaleKoi)
                .WithOrderBy(bp => bp.OrderByDescending(x => x.StartDate))
                .Build());

            var recentBreedingList = recentBreeding
                .Where(bp => bp.StartDate.HasValue)
                .Take(2)
                .ToList();

            foreach (var breeding in recentBreedingList)
            {
                activities.Add(new ActivityFeedDTO
                {
                    Type = "BREEDING",
                    Message = $"Hoàn thành quy trình sinh sản cho cặp #{breeding.Code}",
                    Timestamp = breeding.StartDate ?? now
                });
            }

            return activities
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToList();
        }

        public async Task<FarmQuickStatsDTO> GetQuickStatsAsync()
        {
            var allKoi = await _koiFishRepo.GetAllAsync(new QueryBuilder<KoiFish>().Build());
            var totalKoi = allKoi.Count();
            var healthyKoi = allKoi.Count(k => k.HealthStatus == HealthStatus.Healthy);
            var healthyKoiPercent = totalKoi > 0 ? (double)(healthyKoi / (double)totalKoi * 100) : 0;

            var allPonds = await _pondRepo.GetAllAsync(new QueryBuilder<Pond>().Build());
            var totalPonds = allPonds.Count();
            var activePonds = allPonds.Count(p => p.PondStatus == PondStatus.Active);

            var allUsers = await _userRepo.GetAllAsync(new QueryBuilder<ApplicationUser>()
                .WithPredicate(u => u.Role == Role.FarmStaff || u.Role == Role.Manager || u.Role == Role.SaleStaff)
                .Build());
            var totalStaff = allUsers.Count();
            var activeStaff = allUsers.Count(u => !u.IsDeleted && !u.IsBlocked);

            var activeBreedingProcesses = await _breedingProcessRepo.GetAllAsync(new QueryBuilder<BreedingProcess>()
                .WithPredicate(bp => bp.Status != BreedingStatus.Complete && bp.Status != BreedingStatus.Failed)
                .Build());
            var activeBreedingCount = activeBreedingProcesses.Count();

            return new FarmQuickStatsDTO
            {
                HealthyKoiPercent = Math.Round(healthyKoiPercent, 1),
                ActivePonds = new ActivePondsDTO
                {
                    Current = activePonds,
                    Total = totalPonds
                },
                ActiveStaff = new ActiveStaffDTO
                {
                    Current = activeStaff,
                    Total = totalStaff
                },
                ActiveBreedingProcesses = activeBreedingCount
            };
        }
    }
}

