using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FrySurvivalRecordDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Enums;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Implements
{
    public class FrySurvivalRecordService : IFrySurvivalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<FrySurvivalRecord> _frysurvivalRepo;
        public FrySurvivalRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;   
            _frysurvivalRepo = _unitOfWork.GetRepo<FrySurvivalRecord>();
        }
        public async Task<FrySurvivalRecordResponseDTO> CreateAsync(FrySurvivalRecordRequestDTO dto)
        {
            var _fryfishRepo = _unitOfWork.GetRepo<FryFish>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var fryfish = await _fryfishRepo.GetByIdAsync(dto.FryFishId);
            if (fryfish == null)
            {
                throw new KeyNotFoundException("không tìm thấy bầy cá");
            }
            var breed = await _breedRepo.GetByIdAsync(fryfish.BreedingProcessId);
            if(breed == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản ");
            }
          
            if(dto.CountAlive == 0)
            {
                fryfish.CurrentSurvivalRate = 0;
                fryfish.Status = FryFishStatus.Dead;
                fryfish.EndDate = DateTime.Now;
                breed.Status = BreedingStatus.Failed;
                breed.EndDate = DateTime.Now;
            }
            else if(dto.Success == true) {
                fryfish.Status = FryFishStatus.Completed;
                fryfish.EndDate = DateTime.Now;
            }else
            {
                fryfish.Status = FryFishStatus.Growing;
            }
            var records = await getAllByFryfishId(dto.FryFishId);
            if (records.Any())
            {
                var maxDay = records.Max(r => r.DayNumber);

                if (records.Any(r => r.DayNumber == dto.DayNumber))
                    throw new InvalidOperationException($"DayNumber {dto.DayNumber} đã tồn tại trong EggBatch {dto.FryFishId}.");

                if (dto.DayNumber <= maxDay)
                    throw new InvalidOperationException($"DayNumber {dto.DayNumber} phải lớn hơn {maxDay}.");
            }



            var record = _mapper.Map<FrySurvivalRecord>(dto);
            record.SurvivalRate = ((double)dto.CountAlive / fryfish.InitialCount) * 100;
            fryfish.CurrentSurvivalRate = record.SurvivalRate;
            await _fryfishRepo.UpdateAsync(fryfish);
            await _breedRepo.UpdateAsync(breed);
            await _frysurvivalRepo.CreateAsync(record);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<FrySurvivalRecordResponseDTO>(record);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fryrecord = await _frysurvivalRepo.GetByIdAsync(id);
            if (fryrecord == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ghi nhận cần xóa");
            }

            await _frysurvivalRepo.DeleteAsync(fryrecord);

            return await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<FrySurvivalRecordResponseDTO>> GetAllVarietiesAsync(FrySurvivalRecordFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = new QueryOptions<FrySurvivalRecord>
            {
                IncludeProperties = new List<Expression<Func<FrySurvivalRecord, object>>>
                {
                    p => p.FryFish
                }
            };

            System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => (r.Note != null && r.Note.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.FryFishId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.FryFishId == filter.FryFishId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinDayNumber.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.DayNumber >= filter.MinDayNumber.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxDayNumber.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.DayNumber <= filter.MaxDayNumber.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.SurvivalRate >= filter.MinSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.SurvivalRate <= filter.MaxSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinCountAlive.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.CountAlive >= filter.MinCountAlive.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxCountAlive.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.CountAlive <= filter.MaxCountAlive.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.Success.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.Success == filter.Success.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.CreatedFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.CreatedAt >= filter.CreatedFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.CreatedTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<FrySurvivalRecord, bool>> expr = r => r.CreatedAt <= filter.CreatedTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            query.Predicate = predicate;

            var records = await _frysurvivalRepo.GetAllAsync(query);

            var mappedList = _mapper.Map<List<FrySurvivalRecordResponseDTO>>(records);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<FrySurvivalRecordResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<FrySurvivalRecordResponseDTO?> GetByIdAsync(int id)
        {
            {
                var record = await _frysurvivalRepo.GetSingleAsync(new QueryOptions<FrySurvivalRecord>
                {
                    Predicate = e => e.Id == id,
                    IncludeProperties = new List<Expression<Func<FrySurvivalRecord, object>>>
                {
                    p => p.FryFish
                }
                });
                if (record == null)
                {
                    throw new KeyNotFoundException("không tìm thấy bản ghi nhận");
                }
                return _mapper.Map<FrySurvivalRecordResponseDTO?>(record);
            }
        }

        public async Task<bool> UpdateAsync(int id, FrySurvivalRecordUpdateRequestDTO dto)
        {
            var record = await _frysurvivalRepo.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException("không tìm thấy bản ghi nhận");
            }
            _mapper.Map(dto, record);
            await _frysurvivalRepo.UpdateAsync(record);
            return await _unitOfWork.SaveAsync();
        }
        private async Task<IEnumerable<FrySurvivalRecord>> getAllByFryfishId(int id)
        {
            var queryOptions = new DAL.Queries.QueryOptions<FrySurvivalRecord>
            {
                Predicate = r => r.FryFishId == id,
                OrderBy = q => q.OrderByDescending(r => r.CreatedAt),
                Tracked = false
            };
            var records = await _frysurvivalRepo.GetAllAsync(queryOptions);
            return records;
        }
    }
}
