using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Services.Implements
{
    public class EggBatchService : IEggBatchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<EggBatch> _eggBatchRepo;
        public EggBatchService(IUnitOfWork unitOfWork, IMapper mapper) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();
        }
        public async Task<EggBatchResponseDTO> CreateAsync(EggBatchRequestDTO dto)
        {
            var _breedRepo =  _unitOfWork.GetRepo<BreedingProcess>();
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var breeding = await _breedRepo.GetByIdAsync(dto.BreedingProcessId);
            if (breeding == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản");
            }
            var breedPond = await _pondRepo.GetByIdAsync(breeding.PondId);
            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == dto.PondId , IncludeProperties = new List<Expression<Func<Pond, object>>> { 
                p => p.PondType
                }
            });
            if(pond == null){
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if(pond.PondType.Type != TypeOfPond.EggBatch)
            {
                throw new InvalidOperationException("vui lòng chọn hồ phù hợp với quá trình");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty))
            {
                throw new Exception("hiện tại hồ bạn chọn không trống");
            }
            var eggBacth = new EggBatch
            {
                BreedingProcessId = dto.BreedingProcessId,
                Status = EggBatchStatus.Collected,
                Quantity = dto.Quantity,
                SpawnDate = DateTime.UtcNow 
            };

            // chuyển hồ
            pond.PondStatus = PondStatus.Active;
            breedPond.PondStatus = PondStatus.Empty;
            breeding.PondId = dto.PondId ;
            breeding.TotalEggs = dto.Quantity;
            breeding.Status = BreedingStatus.EggBatch;


            await _breedRepo.UpdateAsync(breeding);
            await _pondRepo.UpdateAsync(pond);
            await _pondRepo.UpdateAsync(breedPond);
            await _eggBatchRepo.CreateAsync(eggBacth);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<EggBatchResponseDTO>(eggBacth);
        }

        public async  Task<bool> DeleteAsync(int id)
        {
            var eggBatch = await _eggBatchRepo.GetByIdAsync(id);
            if (eggBatch == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lô trứng cần xóa");
            }

            await _eggBatchRepo.DeleteAsync(eggBatch);
           
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedList<EggBatchResponseDTO>> GetAllEggBatchAsync(EggBatchFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = new QueryOptions<EggBatch>
            {
            };

            System.Linq.Expressions.Expression<System.Func<EggBatch, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<EggBatch, bool>> expr = e =>
                    e.BreedingProcess != null &&
                    e.BreedingProcess.Pond != null &&
                    e.BreedingProcess.Pond.PondName.Contains(filter.Search);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.BreedingProcessId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.BreedingProcessId == filter.BreedingProcessId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.PondId.HasValue)
            {
                Expression<Func<EggBatch, bool>> expr = e =>
                    e.BreedingProcess != null &&
                    e.BreedingProcess.PondId == filter.PondId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Status.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.Status == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinQuantity.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.Quantity >= filter.MinQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxQuantity.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.Quantity <= filter.MaxQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinFertilizationRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.FertilizationRate >= filter.MinFertilizationRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxFertilizationRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.FertilizationRate <= filter.MaxFertilizationRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.SpawnDateFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.SpawnDate >= filter.SpawnDateFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.SpawnDateTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.SpawnDate <= filter.SpawnDateTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.HatchingTimeFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.HatchingTime >= filter.HatchingTimeFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.HatchingTimeTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<EggBatch, bool>> expr = e => e.HatchingTime <= filter.HatchingTimeTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            query.Predicate = predicate;

            var eggBatch = await _eggBatchRepo.GetAllAsync(query);
            Console.WriteLine(eggBatch.Count());
            var mappedList = _mapper.Map<List<EggBatchResponseDTO>>(eggBatch);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<EggBatchResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<EggBatchResponseDTO?> GetByIdAsync(int id)
        {
            var eggBatch = await _eggBatchRepo.GetSingleAsync(new QueryOptions<EggBatch>
            {
                Predicate = e => e.Id == id,
            });
            if(eggBatch == null)
            {
                throw new KeyNotFoundException("không tìm thấy lô trứng ấp");
            }
            return _mapper.Map<EggBatchResponseDTO?>(eggBatch);
        }

        public async Task<EggBatchResponseDTO> GetEggBatchByBreedId(int breedId)
        {
            var eggBatch = await _eggBatchRepo.GetSingleAsync(new QueryOptions<EggBatch> {
                
                Predicate = b => b.BreedingProcessId == breedId,
                });
            if (eggBatch == null)
            {
                throw new KeyNotFoundException($"không tìm thấy lô trứng với id sinh sản {breedId}");
            }
            return _mapper.Map<EggBatchResponseDTO>(eggBatch);
        }

        public async Task<bool> UpdateAsync(int id, EggBatchUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _eggBatchRepo = _unitOfWork.GetRepo<EggBatch>();

            var eggBatch = await _eggBatchRepo.GetSingleAsync(new QueryOptions<EggBatch>
            {
                Predicate = e => e.Id == id,
                IncludeProperties = new List<Expression<Func<EggBatch, object>>>
        {
            e => e.IncubationDailyRecords
        }
            });

            if (eggBatch == null)
                throw new KeyNotFoundException("Không tìm thấy lô trứng.");

            if (eggBatch.IncubationDailyRecords != null && eggBatch.IncubationDailyRecords.Any())
                throw new InvalidOperationException("Không thể cập nhật lô trứng vì đã có nhật ký ấp.");

            var breed = await _breedRepo.GetByIdAsync(eggBatch.BreedingProcessId);
            if (breed == null)
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản.");

            if (eggBatch.Status == EggBatchStatus.Success || eggBatch.Status == EggBatchStatus.Failed)
                throw new InvalidOperationException($"Lô trứng đã {eggBatch.Status}, không thể cập nhật.");

            
            if (dto.PondId != breed.PondId)
            {
                var newPond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond> { 
                Predicate = p => p.Id == dto.PondId,
                    IncludeProperties = new List<Expression<Func<Pond, object>>>
                    {
                        p => p.PondType
                    }
                });
                if (newPond == null)
                    throw new KeyNotFoundException("Không tìm thấy hồ mới.");
                if(newPond.PondType.Type != TypeOfPond.FryFish)
                {
                    throw new InvalidOperationException("vui lòng chọn hồ phù hợp với quá trình");
                }

                if (newPond.PondStatus == PondStatus.Maintenance || newPond.PondStatus == PondStatus.Active)
                    throw new InvalidOperationException($"Hồ hiện tại đang {newPond.PondStatus}, không thể chuyển lô trứng vào.");

                var oldPond = await _pondRepo.GetByIdAsync(breed.PondId);
                if (oldPond != null)
                    oldPond.PondStatus = PondStatus.Empty;

                newPond.PondStatus = PondStatus.Active;

                await _pondRepo.UpdateAsync(newPond);
                await _pondRepo.UpdateAsync(oldPond);

                breed.PondId = dto.PondId;
            
            }

            _mapper.Map(dto, eggBatch);

            breed.TotalEggs = eggBatch.Quantity;
            await _breedRepo.UpdateAsync(breed);
            await _eggBatchRepo.UpdateAsync(eggBatch);

            return await _unitOfWork.SaveAsync();
        }


        public async Task<bool> CancelEggBatch(int id)
        {
            var eggBacth = await _eggBatchRepo.GetByIdAsync(id);
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var breed = await _breedRepo.GetByIdAsync(eggBacth.BreedingProcessId);
            if (eggBacth == null)
            {
                throw new KeyNotFoundException("không tìm lấy lô trứng");
            }
            if (eggBacth.Status == EggBatchStatus.Success || eggBacth.Status == EggBatchStatus.Failed)
            {
                throw new InvalidOperationException($"hiện tại trạng thái của hồ đã {eggBacth.Status} nên không thể cập nhật");
            }
            var oldPond = await _pondRepo.GetByIdAsync(breed.PondId);
            eggBacth.Status = EggBatchStatus.Failed;
            oldPond.PondStatus = PondStatus.Maintenance;
            breed.Status = BreedingStatus.Failed;
            await _pondRepo.UpdateAsync(oldPond);
            await _breedRepo.UpdateAsync(breed);
            await _eggBatchRepo.UpdateAsync(eggBacth);
            return await _unitOfWork.SaveAsync();
        }
    }
}
