using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationStageDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Enums;
using CloudinaryDotNet.Core;
using Zenkoi.BLL.DTOs.EggBatchDTOs;

namespace Zenkoi.BLL.Services.Implements
{
    public class ClassificationStageService : IClassificationStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<ClassificationStage> _classRepo;
        public ClassificationStageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _classRepo = _unitOfWork.GetRepo<ClassificationStage>();
        }

        public async Task<bool> CompleteClassification(int id)
        {
           var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
           var _pondRepo = _unitOfWork.GetRepo<Pond>();
            
           var classìication = await _classRepo.GetByIdAsync(id);
            if(classìication == null)
            {
                throw new InvalidOperationException("không tìm thấy phân loại");
            }
            if(classìication.Status is ClassificationStatus.Success)
            {
                throw new InvalidOperationException("phân loại đã hoàn thành không thể chỉnh");
            }
            var breed = await _breedRepo.GetByIdAsync(classìication.BreedingProcessId);
            if(breed.Status is BreedingStatus.Failed)
            {
                throw new InvalidOperationException("quá trình sinh sản đã thất bại hoặc hủy nên không thể update");
            }
            var pond = await _pondRepo.GetByIdAsync(breed.PondId);
            pond.PondStatus = PondStatus.Empty;
            breed.PondId = null;
            breed.Status = BreedingStatus.Complete;
            classìication.Status = ClassificationStatus.Success;
            await _breedRepo.UpdateAsync(breed);
            await _classRepo.UpdateAsync(classìication);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<ClassificationStageResponseDTO> CreateAsync(ClassificationStageCreateRequestDTO dto)
        {
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var breed = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess>
            {
                Predicate = b => b.Id == dto.BreedingProcessId,
                IncludeProperties = new List<Expression<Func<BreedingProcess, object>>>
                {
                    a => a.Batch,
                    c => c.FryFish
                }
            });
            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == dto.PondId, IncludeProperties = new List<Expression<Func<Pond, object>>>
                {
                    p => p.PondType
                }
            });
            if (pond == null)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if (pond.PondType.Type  != TypeOfPond.Classification)
            {
                throw new InvalidOperationException("vui lòng chọn hồ phù hợp với quá trình");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty))
            {
                throw new InvalidOperationException("hiện tại hồ bạn chọn không trống");
            }
            var currentFish = (breed.FryFish.CurrentSurvivalRate/100) * breed.FryFish.InitialCount;
            if(pond.MaxFishCount < currentFish)
            {
                throw new InvalidOperationException("số lượng cá vượt sức chứa của hồ");
            }

            if (breed == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản");
            }
            if (breed.FryFish ==null || !breed.FryFish.Status.Equals(FryFishStatus.Completed) && !breed.FryFish.Status.Equals(FryFishStatus.Growing) )
            {
                throw new InvalidOperationException("Quá trình nuôi cá bột chưa hoàn thành không thể tạo phân loại");
            }
            var _fryRepo = _unitOfWork.GetRepo<FryFish>();
            var fryFish = await _fryRepo.GetByIdAsync(breed.FryFish.Id);
            var fryPond = await _pondRepo.GetByIdAsync(breed.PondId);

            // chuyen ho
            fryPond.PondStatus = PondStatus.Empty;
            pond.PondStatus = PondStatus.Active;
            fryFish.Status = FryFishStatus.Completed;
            breed.Status = BreedingStatus.Classification; 

            var classification = _mapper.Map<ClassificationStage>(dto);
            classification.Status = ClassificationStatus.Preparing;
            classification.TotalCount = (int)(fryFish.InitialCount * (fryFish.CurrentSurvivalRate / 100.0));
            await _breedRepo.UpdateAsync(breed);
            await _classRepo.CreateAsync(classification);
            await _unitOfWork.SaveChangesAsync();
           return _mapper.Map<ClassificationStageResponseDTO>(classification);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var classification = await _classRepo.GetByIdAsync(id);
            if (classification == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phân loại cần xóa");
            }

            await _classRepo.DeleteAsync(classification);

            return await _unitOfWork.SaveAsync();
        }

        public async  Task<PaginatedList<ClassificationStageResponseDTO>> GetAllAsync(ClassificationStageFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = new QueryOptions<ClassificationStage>
            {
  
            };

            System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>>? predicate = null;
            if (!string.IsNullOrEmpty(filter.Search))
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => (r.Notes != null && r.Notes.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.BreedingProcessId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => r.BreedingProcessId == filter.BreedingProcessId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.PondId.HasValue)
            {
                Expression<Func<ClassificationStage, bool>> expr = e =>
                    e.BreedingProcess != null &&
                    e.BreedingProcess.PondId == filter.PondId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.Status.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => r.Status == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinTotalCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => r.TotalCount >= filter.MinTotalCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxTotalCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => r.TotalCount <= filter.MaxTotalCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            // other count filters if needed...

            query.Predicate = predicate;

            var classifications = await _classRepo.GetAllAsync(query);

            var mappedList = _mapper.Map<List<ClassificationStageResponseDTO>>(classifications);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<ClassificationStageResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<ClassificationStageResponseDTO?> GetByIdAsync(int id)
        {
            
            var classifications = await _classRepo.GetSingleAsync(new QueryOptions<ClassificationStage>
            {
                Predicate = e => e.Id == id,
            });
            if (classifications == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá phân loại");
            }
           

            return _mapper.Map<ClassificationStageResponseDTO>(classifications);
        }

        public  async Task<ClassificationStageResponseDTO> GetClassificationStageByBreedId(int breedId)
        {
            var classìication = await _classRepo.GetSingleAsync(new QueryOptions<ClassificationStage>
            {

                Predicate = b => b.BreedingProcessId == breedId,
            });
            if (classìication == null)
            {
                throw new KeyNotFoundException($"không tìm thấy lô cá phân loại với id sinh sản {breedId}");
            }
            return _mapper.Map<ClassificationStageResponseDTO>(classìication);
        }

        public async Task<bool> UpdateAsync(int id, ClassificationStageUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _classRepo = _unitOfWork.GetRepo<ClassificationStage>();

            var classification = await _classRepo.GetSingleAsync(new QueryOptions<ClassificationStage>
            {
                Predicate = c => c.Id == id,
                IncludeProperties = new List<Expression<Func<ClassificationStage, object>>>
        {
            c => c.ClassificationRecords
        }
            });

            if (classification == null)
                throw new KeyNotFoundException("Không tìm thấy bầy phân loại.");

            if (classification.ClassificationRecords != null && classification.ClassificationRecords.Any())
                throw new InvalidOperationException("Không thể cập nhật vì bầy đã có ghi nhận phân loại.");

            if (classification.Status == ClassificationStatus.Success)
                throw new InvalidOperationException($"Bầy phân loại đã ở trạng thái {classification.Status}, không thể cập nhật.");

            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (breed == null)
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản.");


            if (dto.PondId == breed.PondId)
            {
                _mapper.Map(dto, classification);
                await _classRepo.UpdateAsync(classification);

                return await _unitOfWork.SaveAsync();
            }

            var newPond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == id, IncludeProperties = new List<Expression<Func<Pond, object>>>
                {
                    p => p.PondType
                }
            });
            if (newPond == null)
                throw new KeyNotFoundException("Không tìm thấy hồ mới.");
            if (newPond.PondType.Type != TypeOfPond.Classification)
            {
                throw new InvalidOperationException("vui lòng chọn hồ phù hợp với quá trình");
            }

            if (newPond.PondStatus == PondStatus.Maintenance || newPond.PondStatus == PondStatus.Active)
                throw new InvalidOperationException($"Hồ hiện đang {newPond.PondStatus}, không thể chuyển bầy vào.");

            var oldPond = await _pondRepo.GetByIdAsync(breed.PondId);
            if (oldPond == null)
                throw new KeyNotFoundException("Không tìm thấy hồ cũ.");

            oldPond.PondStatus = PondStatus.Empty;
            newPond.PondStatus = PondStatus.Active;

            await _pondRepo.UpdateAsync(oldPond);
            await _pondRepo.UpdateAsync(newPond);

            breed.PondId = dto.PondId;
            await _breedRepo.UpdateAsync(breed);

            _mapper.Map(dto, classification);
            await _classRepo.UpdateAsync(classification);

            return await _unitOfWork.SaveAsync();
        }

    }
}
