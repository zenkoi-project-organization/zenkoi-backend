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
            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if (pond == null)
            {
                throw new Exception("không tìm thấy hồ");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty))
            {
                throw new Exception("hiện tại hồ bạn chọn không trống");
            }
            if (breed == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản");
            }
            if (breed.FryFish ==null ||!breed.FryFish.Status.Equals(FryFishStatus.Completed))
            {
                throw new Exception("Quá trình nuôi cá bột chưa hoàn thành không thể tạo phân loại");
            }
            var _fryRepo = _unitOfWork.GetRepo<FryFish>();
            var fryFish = await _fryRepo.GetByIdAsync(breed.FryFish.Id);
            var fryPond = await _pondRepo.GetByIdAsync(fryFish.PondId);

            // chuyen ho
            fryFish.PondId = null;
            fryPond.PondStatus = PondStatus.Empty;
            pond.PondStatus = PondStatus.Active;
            breed.Status = BreedingStatus.Classification; 

            var classification = _mapper.Map<ClassificationStage>(dto);
            classification.Status = ClassificationStatus.Preparing;
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
                IncludeProperties = new List<Expression<Func<ClassificationStage, object>>>
                {
                    p => p.Pond
                }
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
                System.Linq.Expressions.Expression<System.Func<ClassificationStage, bool>> expr = r => r.PondId == filter.PondId.Value;
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
                IncludeProperties = new List<Expression<Func<ClassificationStage, object>>>
                {
                    p => p.Pond
                }
            });
            if (classifications == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá phân loại");
            }
           

            return _mapper.Map<ClassificationStageResponseDTO>(classifications);
        }

        public async Task<bool> UpdateAsync(int id, ClassificationStageUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var pond = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pond)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            var classifications = await _classRepo.GetByIdAsync(id);
            if (classifications == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
            }
            _mapper.Map(dto, classifications);
            await _classRepo.UpdateAsync(classifications);
            return await _unitOfWork.SaveAsync();
        }
    }
}
