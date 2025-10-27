using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Enums;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Paging;
using Zenkoi.BLL.Services.Interfaces;
namespace Zenkoi.BLL.Services.Implements
{
    public class FryFishService : IFryFishService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<FryFish> _fryFishRepo;
        public FryFishService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fryFishRepo = _unitOfWork.GetRepo<FryFish>();
        }
        public async Task<FryFishResponseDTO> CreateAsync(FryFishRequestDTO dto)
        {
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _eggRepo = _unitOfWork.GetRepo<EggBatch>();
            var breeding = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess> {
                Predicate = b => b.Id == dto.BreedingProcessId ,
            IncludeProperties = new List<Expression<Func<BreedingProcess, object>>> { 
                e => e.Batch,
                f => f.FryFish
            }
            });
            if (breeding == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản");
            }
            if(breeding.Batch == null )
            {
                throw new Exception("Quy trình sinh sản chưa có ấp trứng nên không thể tạo nuôi cá bột");
            }
            if(!breeding.Batch.Status.Equals(EggBatchStatus.Success) && !breeding.Batch.Status.Equals(EggBatchStatus.PartiallyHatched))
            {
                throw new Exception("Ấp trứng chưa hoàn thành nên không thể tạo nuôi cá bột");
            }
            if(breeding.FryFish != null)
            {
                throw new Exception("Quy trình sinh sản đã có nuôi cá bột");
            }


            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if (pond == null)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty))
            {
                throw new Exception("hiện tại hồ bạn chọn không trống");
            }
            var eggBatch = await _eggRepo.GetByIdAsync(breeding.Batch.Id);
            var eggPond = await _pondRepo.GetByIdAsync(breeding.PondId);

            // chuyển hồ
            eggPond.PondStatus = PondStatus.Empty;
            breeding.PondId = dto.PondId;
            breeding.Status = BreedingStatus.FryFish;
            pond.PondStatus = PondStatus.Active;
            var fryFish = _mapper.Map<FryFish>(dto);
            fryFish.InitialCount = eggBatch.TotalHatchedEggs;
            fryFish.Status = FryFishStatus.Hatched;
            fryFish.StartDate = DateTime.Now;
            await _pondRepo.UpdateAsync(eggPond);
            await _pondRepo.UpdateAsync(pond);
            await _eggRepo.UpdateAsync(eggBatch);
            await _fryFishRepo.CreateAsync(fryFish);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<FryFishResponseDTO>(fryFish);
        }

        public async  Task<bool> DeleteAsync(int id)
        {
            var fryfish = await _fryFishRepo.GetByIdAsync(id);
            if (fryfish == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lô trứng cần xóa");
            }

            await _fryFishRepo.DeleteAsync(fryfish);

            return await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<FryFishResponseDTO>> GetAllAsync(FryFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = new QueryOptions<FryFish>
            {
            };

            System.Linq.Expressions.Expression<System.Func<FryFish, bool>>? predicate = null;
            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<FryFish, bool>> expr = e =>
                    e.BreedingProcess != null &&
                    e.BreedingProcess.Pond != null &&
                    e.BreedingProcess.Pond.PondName.Contains(filter.Search);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.BreedingProcessId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.BreedingProcessId == filter.BreedingProcessId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.PondId.HasValue)
            {
                Expression<Func<FryFish, bool>> expr = e =>
                    e.BreedingProcess != null &&
                    e.BreedingProcess.PondId == filter.PondId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Status.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.Status == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinInitialCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.InitialCount >= filter.MinInitialCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxInitialCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.InitialCount <= filter.MaxInitialCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinCurrentSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.CurrentSurvivalRate >= filter.MinCurrentSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxCurrentSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.CurrentSurvivalRate <= filter.MaxCurrentSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.StartDateFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.StartDate >= filter.StartDateFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.StartDateTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.StartDate <= filter.StartDateTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.EndDateFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.EndDate >= filter.EndDateFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.EndDateTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FryFish, bool>> expr = e => e.EndDate <= filter.EndDateTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            query.Predicate = predicate;

            var fryfish = await _fryFishRepo.GetAllAsync(query);

            var mappedList = _mapper.Map<List<FryFishResponseDTO>>(fryfish);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<FryFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<FryFishResponseDTO?> GetByIdAsync(int id)
        {
            var fryFish = await _fryFishRepo.GetSingleAsync(new QueryOptions<FryFish>
            {
                Predicate = e => e.Id == id,
                IncludeProperties = new List<Expression<Func<FryFish, object>>>
                {
                    r => r.FrySurvivalRecords 
                }
            });
            if (fryFish == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
            }
            return _mapper.Map<FryFishResponseDTO?>(fryFish);
    }

        public async Task<FryFishResponseDTO> GetEggBatchByBreedId(int breedId)
        {
            var fryFish = await _fryFishRepo.GetSingleAsync(new QueryOptions<FryFish>
            {

                Predicate = b => b.BreedingProcessId == breedId,
            });
            if (fryFish == null)
            {
                throw new KeyNotFoundException($"không tìm thấy lô cá bột với id sinh sản {breedId}");
            }
            return _mapper.Map<FryFishResponseDTO>(fryFish);
        }

        public async Task<FrySurvivalSummaryDTO> GetFrySurvivalSummaryAsync(int fryFishId)
        {
            var fryfishRepo = _unitOfWork.GetRepo<FryFish>();
            var fryfish = await fryfishRepo.GetSingleAsync(new QueryOptions<FryFish>
            {
                Predicate = f => f.Id == fryFishId,
                IncludeProperties = new List<Expression<Func<FryFish, object>>> { f => f.FrySurvivalRecords, f => f.BreedingProcess }
            });

            if (fryfish == null)
                throw new KeyNotFoundException("Không tìm thấy bầy cá");

            var records = fryfish.FrySurvivalRecords.OrderBy(r => r.DayNumber).ToList();
            if (!records.Any())
                throw new InvalidOperationException("Chưa có ghi nhận cá bột");

            DateTime start = fryfish.StartDate != default(DateTime)
             ? fryfish.StartDate.Date
             : records.First().DayNumber.Value.Date;

            double? GetRateAtDay(int days)
            {
                var targetDate = start.AddDays(days);

                if (targetDate > DateTime.Now)
                    return null;

                var record = records
                    .Where(r => r.DayNumber <= targetDate)
                    .OrderByDescending(r => r.DayNumber)
                    .FirstOrDefault();

                return record?.SurvivalRate;
            }

            var summary = new FrySurvivalSummaryDTO
            {
                BreedingProcessCode = fryfish.BreedingProcess.Code,
                MaleKoi = fryfish.BreedingProcess.MaleKoiId,
                FemaleKoi = fryfish.BreedingProcess.FemaleKoiId,
                PondName = fryfish.BreedingProcess.Pond?.PondName,
                StartDate = fryfish.StartDate,
                InitialCount = fryfish.InitialCount ?? 0,
                SurvivalRate7Days = GetRateAtDay(7),
                SurvivalRate14Days = GetRateAtDay(14),
                SurvivalRate30Days = GetRateAtDay(30),
                CurrentRate = records.Last().SurvivalRate
            };

            return summary;
        }
    

    public async  Task<bool> UpdateAsync(int id, FryFishUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var pond = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pond)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            var fryFish = await _fryFishRepo.GetByIdAsync(id);
            if(fryFish == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
            }
            _mapper.Map(dto, fryFish);
            await _fryFishRepo.UpdateAsync(fryFish);
            return  await _unitOfWork.SaveAsync();
        }
    }
}
