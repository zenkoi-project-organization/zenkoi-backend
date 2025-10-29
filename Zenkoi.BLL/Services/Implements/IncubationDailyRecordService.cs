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
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;


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
          
            if (eggBatch.Quantity < (dto.HatchedEggs + dto.HealthyEggs ))
            {
                throw new Exception("tổng số bạn nhập lớn hơn so với lô trứng ghi nhận");
            } 

            if (eggBatch.TotalHatchedEggs ==0 && dto.HatchedEggs > 0)
            {
                eggBatch.Status = EggBatchStatus.PartiallyHatched;
                eggBatch.HatchingTime = DateTime.Now;
            }

            var record = _mapper.Map<IncubationDailyRecord>(dto);
            record.RottenEggs = eggBatch.Quantity - (dto.HatchedEggs + dto.HealthyEggs);
            await _incubationDailyRepo.CreateAsync(record);
            eggBatch.TotalHatchedEggs += dto.HatchedEggs;
            Console.WriteLine($"TONG TRUNG : {dto.HealthyEggs} SO LUONG : {eggBatch.Quantity}");
            eggBatch.FertilizationRate = (double)dto.HealthyEggs / eggBatch.Quantity * 100;
            breed.FertilizationRate = eggBatch.FertilizationRate;
            if (record.Success)
            {
                eggBatch.Status = EggBatchStatus.Success;
                eggBatch.SpawnDate = DateTime.Now;
                eggBatch.EndDate = DateTime.Now;

            }
            await _eggBatchRepo.UpdateAsync(eggBatch);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<IncubationDailyRecordResponseDTO>(record);
            
        }

        public async Task<IncubationDailyRecordResponseDTO> CreateV2Async(IncubationDailyRecordRequestV2DTO dto)
        {
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var eggBatch = await _eggBatchRepo.GetByIdAsync(dto.EggBatchId);
           
            if (eggBatch == null)
            {
                throw new KeyNotFoundException("Không tim thấy lô trứng ");
            }
            var breed = await _breedRepo.GetByIdAsync(eggBatch.BreedingProcessId);
            if(breed == null){
                throw new KeyNotFoundException("Không tim thấy quy trinh sinh sản ");

            }

            if (eggBatch.Status.Equals(EggBatchStatus.Success) || eggBatch.Status.Equals(EggBatchStatus.Failed))
            {

                throw new KeyNotFoundException($"Lô trứng đã {eggBatch.Status}");
            }

            var records = await getAllbyEggBatchId(eggBatch.Id);
            var total = await GetSummaryByEggBatchIdAsync(eggBatch.Id);
            var record = _mapper.Map<IncubationDailyRecord>(dto);
            if (records.Any())
            {
                var lastRecord = records
                .OrderByDescending(r => r.DayNumber)
                .First();

                if(dto.HatchedEggs  > lastRecord.HealthyEggs)
                {
                    throw new InvalidOperationException("tổng trứng nở lớn hơn số trứng khỏe ở lần nhập trước");
                }
              
                record.HealthyEggs = lastRecord.HealthyEggs - dto.HatchedEggs;
                if (record.Success)
                {
                    eggBatch.Status = EggBatchStatus.Success;
                    eggBatch.SpawnDate = DateTime.Now;
                    eggBatch.EndDate = DateTime.Now;
                    record.RottenEggs = eggBatch.Quantity - (total.TotalHatchedEggs + dto.HatchedEggs);
                    eggBatch.TotalHatchedEggs = total.TotalHatchedEggs + dto.HatchedEggs;
                    breed.HatchingRate = (double)eggBatch.TotalHatchedEggs / eggBatch.Quantity * 100;
                }
                await _incubationDailyRepo.CreateAsync(record);
                await _eggBatchRepo.UpdateAsync(eggBatch);
                await _unitOfWork.SaveChangesAsync();
            }
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

        public async Task<bool> UpdateAsync(int id, IncubationDailyRecordUpdateRequestDTO dto)
        {
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();

            var record = await _incubationDailyRepo.GetByIdAsync(id);
            if (record == null)
                throw new KeyNotFoundException("Không tìm thấy ghi nhận cần cập nhật");

            var eggBatch = await _eggBatchRepo.GetByIdAsync(record.EggBatchId);
            if (eggBatch == null)
                throw new KeyNotFoundException("Không tìm thấy lô trứng");

       
            var latestRecord = await _incubationDailyRepo.GetSingleAsync(new QueryOptions<IncubationDailyRecord>
            {
                Predicate = r => r.EggBatchId == eggBatch.Id,
                OrderBy = q => q.OrderByDescending(r => r.DayNumber),
                Tracked = false
            });

            if (latestRecord == null || latestRecord.Id != id)
                throw new InvalidOperationException("Chỉ có thể cập nhật bản ghi mới nhất.");

            if (eggBatch.Status is EggBatchStatus.Success or EggBatchStatus.Failed)
                throw new InvalidOperationException($"Lô trứng đã {eggBatch.Status}, không thể cập nhật");

            var total = (dto.HealthyEggs ?? 0) + (dto.HatchedEggs ?? 0);
            if (total > eggBatch.Quantity)
                throw new InvalidOperationException("Tổng trứng khỏe + trứng nở vượt quá số lượng lô");

            record.HealthyEggs = dto.HealthyEggs;
            record.HatchedEggs = dto.HatchedEggs;
            record.Success = dto.Success;
            record.RottenEggs = eggBatch.Quantity - total;

            if (record.Success)
            {
                eggBatch.Status = EggBatchStatus.Success;
                eggBatch.SpawnDate = DateTime.Now;
                eggBatch.EndDate = DateTime.Now;
            }

            eggBatch.FertilizationRate = (double)(dto.HealthyEggs ?? 0) / eggBatch.Quantity * 100;

            var breed = await _breedRepo.GetByIdAsync(eggBatch.BreedingProcessId);
            if (breed != null)
            {
                breed.FertilizationRate = eggBatch.FertilizationRate;
                await _breedRepo.UpdateAsync(breed);
            }

            await _incubationDailyRepo.UpdateAsync(record);
            await _eggBatchRepo.UpdateAsync(eggBatch);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateV2Async(int id, IncubationDailyRecordUpdateV2RequestDTO dto)
        {
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();

            var record = await _incubationDailyRepo.GetByIdAsync(id);
            if (record == null)
                throw new KeyNotFoundException("Không tìm thấy ghi nhận cần cập nhật");

            var eggBatch = await _eggBatchRepo.GetByIdAsync(record.EggBatchId);
            if (eggBatch == null)
                throw new KeyNotFoundException("Không tìm thấy lô trứng");

            var breed = await _breedRepo.GetByIdAsync(eggBatch.BreedingProcessId);
            if (breed == null)
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản");

          
            var latestRecord = await _incubationDailyRepo.GetSingleAsync(new QueryOptions<IncubationDailyRecord>
            {
                Predicate = r => r.EggBatchId == eggBatch.Id,
                OrderBy = q => q.OrderByDescending(r => r.DayNumber),
                Tracked = false
            });

            if (latestRecord == null || latestRecord.Id != id)
                throw new InvalidOperationException("Chỉ có thể cập nhật bản ghi mới nhất.");

            if (eggBatch.Status == EggBatchStatus.Success || eggBatch.Status == EggBatchStatus.Failed)
                throw new InvalidOperationException($"Lô trứng đã {eggBatch.Status}, không thể cập nhật");

            var allRecords = await getAllbyEggBatchId(eggBatch.Id);
            var totalBefore = await GetSummaryByEggBatchIdAsync(eggBatch.Id);

            var previousRecord = allRecords
                .Where(r => r.Id != id)
                .OrderByDescending(r => r.DayNumber)
                .FirstOrDefault();

            if (previousRecord != null && dto.HatchedEggs > previousRecord.HealthyEggs)
                throw new InvalidOperationException("Số trứng nở mới vượt quá số trứng khỏe còn lại của lần nhập trước");

            record.HatchedEggs = dto.HatchedEggs;
            record.Success = dto.Success;

            if (previousRecord != null)
            {
                record.HealthyEggs = previousRecord.HealthyEggs - dto.HatchedEggs;
                if (record.HealthyEggs < 0)
                    throw new InvalidOperationException("Số trứng khỏe còn lại không thể âm");
            }
            else
            {
                record.HealthyEggs = eggBatch.Quantity - dto.HatchedEggs;
            }

            record.RottenEggs = eggBatch.Quantity - (totalBefore.TotalHatchedEggs + dto.HatchedEggs + record.HealthyEggs);
            if (record.RottenEggs < 0)
                record.RottenEggs = 0;

            if (record.Success)
            {
                eggBatch.Status = EggBatchStatus.Success;
                eggBatch.SpawnDate = DateTime.Now;
                eggBatch.EndDate = DateTime.Now;

                eggBatch.TotalHatchedEggs = totalBefore.TotalHatchedEggs + dto.HatchedEggs;
                breed.HatchingRate = (double)eggBatch.TotalHatchedEggs / eggBatch.Quantity * 100;
            }

            await _incubationDailyRepo.UpdateAsync(record);
            await _eggBatchRepo.UpdateAsync(eggBatch);
            await _breedRepo.UpdateAsync(breed);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }



        public async Task<EggBatchSummaryDTO> GetSummaryByEggBatchIdAsync(int eggBatchId)
        {
            var _recordRepo = _unitOfWork.GetRepo<IncubationDailyRecord>();
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
            var queryOptions = new QueryOptions<IncubationDailyRecord>
            {
                Predicate = r => r.EggBatchId == eggBatchId,
                Tracked = false
            };

            var records = await _recordRepo.GetAllAsync(queryOptions);
            var eggBatch = await _eggBatchRepo.GetByIdAsync(eggBatchId);
            if (records == null || !records.Any())
            {
                return new EggBatchSummaryDTO
                {
                    EggBatchId = eggBatchId,
                    FertilizationRate = 0,
                    HealthyEggs = 0,
                    TotalRottenEggs = 0,
                    TotalHatchedEggs = 0
                };
            }

            var totalRotten = records.Sum(r => r.RottenEggs ?? 0);
            var totalHatched = records.Sum(r => r.HatchedEggs ?? 0);
            var totalHealthy = records.Sum(r => r.HealthyEggs ?? 0);

            return new EggBatchSummaryDTO
            {
                EggBatchId = eggBatchId,
                FertilizationRate = (totalHealthy / eggBatch.Quantity) * 100,
                TotalRottenEggs = totalRotten,
                TotalHatchedEggs = totalHatched,
            };
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
                OrderBy = q => q.OrderByDescending(r => r.DayNumber),
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
  
            if (last.HatchedEggs > dto.HatchedEggs)
                throw new InvalidOperationException("Số trứng nở không thể giảm so với ngày trước.");
        }
    }
}
