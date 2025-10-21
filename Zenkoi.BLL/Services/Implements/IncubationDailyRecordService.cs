using AutoMapper;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.IncubationDailyRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;


namespace Zenkoi.BLL.Services.Implements
{
    public class IncubationDailyRecordService : IIncubationDailyRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<IncubationDailyRecord> _incubationDailyRepo;
        public IncubationDailyRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _incubationDailyRepo = _unitOfWork.GetRepo<IncubationDailyRecord>();
        }
        public async Task<IncubationDailyRecordResponseDTO> CreateAsync(IncubationDailyRecordRequestDTO dto)
        {
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var eggBatch = await _eggBatchRepo.GetByIdAsync(dto.EggBatchId);
            if(eggBatch == null)
            {
                throw new KeyNotFoundException("Không tim thấy lô trứng ");
            }
            if (eggBatch.Status.Equals(EggBatchStatus.Success) || eggBatch.Status.Equals(EggBatchStatus.Failed))
            {

                throw new KeyNotFoundException($"Lô trứng đã {eggBatch.Status}");
            }
            var breed = await _breedRepo.GetByIdAsync(eggBatch.BreedingProcessId);
            // validate nhập liệu
            var lastRecord = await GetLatestRecordByEggBatchIdAsync(dto.EggBatchId);
            if (lastRecord != null)
            {
                ValidateConsistencyWithLastRecord(lastRecord, dto);
            }

            if (eggBatch.Quantity < (dto.HatchedEggs + dto.HealthyEggs + dto.RottenEggs))
            {
                throw new Exception("tổng số bạn nhập lớn hơn so với lô trứng ghi nhận");
            } 
             
            var records = await getAllbyEggBatchId(eggBatch.Id);
            if (records.Any())
            {
                var maxDay = records.Max(r => r.DayNumber);

                if (records.Any(r => r.DayNumber == dto.DayNumber))
                    throw new InvalidOperationException($"DayNumber {dto.DayNumber} đã tồn tại trong EggBatch {dto.EggBatchId}.");

                if (dto.DayNumber <= maxDay)
                    throw new InvalidOperationException($"DayNumber {dto.DayNumber} phải lớn hơn {maxDay}.");
            }

            if (eggBatch.TotalHatchedEggs ==0 && dto.HatchedEggs > 0)
            {
                eggBatch.Status = EggBatchStatus.PartiallyHatched;
            }

            var record = _mapper.Map<IncubationDailyRecord>(dto);
            await _incubationDailyRepo.CreateAsync(record);
            eggBatch.TotalHatchedEggs += dto.HatchedEggs;
            if(record.Success)
            {
                eggBatch.Status = EggBatchStatus.Success;
                eggBatch.SpawnDate = DateTime.Now;
                breed.FertilizationRate = eggBatch.FertilizationRate;

                var avgHealthyEggs = eggBatch.IncubationDailyRecords
                .OrderBy(r => r.DayNumber)
                .Take(3)
                .Where(r => r.HealthyEggs.HasValue)
                .Average(r => r.HealthyEggs.Value);
                eggBatch.FertilizationRate = (avgHealthyEggs / eggBatch.Quantity.Value) * 100;

            }
            await _eggBatchRepo.UpdateAsync(eggBatch);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<IncubationDailyRecordResponseDTO>(record);
            
        }

        public async Task<bool> DeleteAsync(int id)
        {
            {
                var record = await _incubationDailyRepo.GetByIdAsync(id);
                if (record == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy ghi nhận cần xóa");
                }

                await _incubationDailyRepo.DeleteAsync(record);

                return await _unitOfWork.SaveAsync();
            }
        }

        public async Task<PaginatedList<IncubationDailyRecordResponseDTO>> GetAllByEggBatchIdAsync(int eggBatchId, int pageIndex = 1, int pageSize = 10)
        {

            var records = await getAllbyEggBatchId(eggBatchId);
            var mappedList = _mapper.Map<List<IncubationDailyRecordResponseDTO>>(records);
          
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<IncubationDailyRecordResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<IncubationDailyRecordResponseDTO?> GetByIdAsync(int id)
        {
            var record = await _incubationDailyRepo.GetSingleAsync(new QueryOptions<IncubationDailyRecord>
            {
                IncludeProperties = new List<Expression<Func<IncubationDailyRecord, object>>>
                {
                    b => b.EggBatch
                }
            });
            return _mapper.Map<IncubationDailyRecordResponseDTO?>(record);
        }

        public async Task<bool> UpdateAsync(int id, IncubationDailyRecordRequestDTO dto)
        {
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var eggBatch = await _eggBatchRepo.GetByIdAsync(dto.EggBatchId);
            if (eggBatch == null)
            {
                throw new KeyNotFoundException("Không tim thấy lô trứng ");
            }
            if (eggBatch.Status.Equals(EggBatchStatus.Success) || eggBatch.Status.Equals(EggBatchStatus.Failed))
            {

                throw new Exception($"Lô trứng đã {eggBatch.Status}");
            }
            var record = _mapper.Map<IncubationDailyRecord>(dto);
            await _incubationDailyRepo.UpdateAsync(record);

            if (record.Success)
            {
                eggBatch.Status = EggBatchStatus.Success;
                await _eggBatchRepo.UpdateAsync(eggBatch);
            }
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        private async Task<IEnumerable<IncubationDailyRecord>> getAllbyEggBatchId(int id)
        {
            var queryOptions = new DAL.Queries.QueryOptions<IncubationDailyRecord>
            {
                Predicate = r => r.EggBatchId == id,
                IncludeProperties = new List<Expression<Func<IncubationDailyRecord, object>>>
                 {
                     r => r.EggBatch
                 },
                OrderBy = q => q.OrderByDescending(r => r.CreatedAt),
                Tracked = false
            };
            var records = await _incubationDailyRepo.GetAllAsync(queryOptions);
            return records;
        }
        private async Task<IncubationDailyRecord?> GetLatestRecordByEggBatchIdAsync(int eggBatchId)
        {
            var queryOptions = new QueryOptions<IncubationDailyRecord>
            {
                Predicate = r => r.EggBatchId == eggBatchId,
                OrderBy = q => q.OrderByDescending(r => r.DayNumber),
                Tracked = false
            };
            var record = await _incubationDailyRepo.GetSingleAsync(queryOptions);
            return record;
        }
        private void ValidateConsistencyWithLastRecord(IncubationDailyRecord last, IncubationDailyRecordRequestDTO dto)
        {
            if (last.HealthyEggs < dto.HealthyEggs)
                throw new InvalidOperationException("Số trứng khỏe không thể tăng so với ngày trước.");
            if (last.RottenEggs > dto.RottenEggs)
                throw new InvalidOperationException("Số trứng hỏng không thể giảm so với ngày trước.");
            if (last.HatchedEggs > dto.HatchedEggs)
                throw new InvalidOperationException("Số trứng nở không thể giảm so với ngày trước.");
        }
    }
}
