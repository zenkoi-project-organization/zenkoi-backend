using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class ClassificationRecordService : IClassificationRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<ClassificationRecord> _recordRepo;
        public ClassificationRecordService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _recordRepo = _unitOfWork.GetRepo<ClassificationRecord>();
        }

        // phân loại lần 1 
        public async Task<ClassificationRecordResponseDTO> CreateAsync(ClassificationRecordRequestDTO dto)
        {
            var _classRepo = _unitOfWork.GetRepo<ClassificationStage>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
           
            var classification = await _classRepo.GetByIdAsync(dto.ClassificationStageId);
            if(classification == null)
            {
                throw new KeyNotFoundException("không tìm thấy bầy phân loại");
            }
            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status.Equals(ClassificationStatus.Success)){
                throw new Exception("Phân loại đã hoàn thành không thể tạo thêm ghi nhận nữa");
            }

            var hasAnyRecord = await _recordRepo.AnyAsync(new QueryOptions<ClassificationRecord>
            {
                Predicate = r => r.ClassificationStageId == dto.ClassificationStageId
            });

            var record = _mapper.Map<ClassificationRecord>(dto);

            if (!hasAnyRecord)
            {
                // 🟢 Đây là lần đầu tiên
                record.StageNumber = 1;
                record.PondQualifiedCount = classification.TotalCount - dto.CullQualifiedCount;
                classification.Status = (ClassificationStatus)record.StageNumber;
                classification.PondQualifiedCount = record.PondQualifiedCount;

            }
            else
            {
                record.StageNumber = (int)classification.Status + 1;
                record.PondQualifiedCount = classification.TotalCount - dto.CullQualifiedCount;
                classification.Status = (ClassificationStatus)record.StageNumber;
                classification.PondQualifiedCount = record.PondQualifiedCount;
            }

            if (record.StageNumber == 4)
            {
                classification.Status = ClassificationStatus.Success;
                breed.Status = BreedingStatus.Complete;
                breed.EndDate = DateTime.Now;
            }
            if(record.StageNumber == 5)
            {
                throw new Exception("bạn đã hoàn thành quy trình phân ");
            }

            await _recordRepo.CreateAsync(record);
            await _classRepo.UpdateAsync(classification);
            await _breedRepo.UpdateAsync(breed);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClassificationRecordResponseDTO>(record);
        }
        // phân loại lần 3
        public async Task<ClassificationRecordResponseDTO> CreateV2Async(ClassificationRecordV2RequestDTO dto)
        {
            var _classRepo = _unitOfWork.GetRepo<ClassificationStage>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();

            var classification = await _classRepo.GetByIdAsync(dto.ClassificationStageId);
            if (classification == null)
            {
                throw new KeyNotFoundException("không tìm thấy bầy phân loại");
            }
            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status.Equals(ClassificationStatus.Success))
            {
                throw new Exception("Phân loại đã hoàn thành không thể tạo thêm ghi nhận nữa");
            }

            var record = _mapper.Map<ClassificationRecord>(dto);
            record.PondQualifiedCount = classification.PondQualifiedCount - dto.ShowQualifiedCount;
            record.StageNumber += (int)classification.Status + 1;
            classification.Status = (ClassificationStatus)record.StageNumber;
            classification.ShowQualifiedCount = record.ShowQualifiedCount;

            if (record.StageNumber == 4)
            {
                classification.Status = ClassificationStatus.Success;
                breed.Status = BreedingStatus.Complete;
                breed.EndDate = DateTime.Now;
            }
            if (record.StageNumber == 5)
            {
                throw new Exception("bạn đã hoàn thành quy trình phân ");
            }

            await _recordRepo.CreateAsync(record);
            await _classRepo.UpdateAsync(classification);
            await _breedRepo.UpdateAsync(breed);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClassificationRecordResponseDTO>(record);
        } 
        // phân loại lần cuối
        public async Task<ClassificationRecordResponseDTO> CreateV3Async(ClassificationRecordV3RequestDTO dto)
        {
            var _classRepo = _unitOfWork.GetRepo<ClassificationStage>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();

            var classification = await _classRepo.GetByIdAsync(dto.ClassificationStageId);
            if (classification == null)
            {
                throw new KeyNotFoundException("không tìm thấy bầy phân loại");
            }
            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status.Equals(ClassificationStatus.Success))
            {
                throw new Exception("Phân loại đã hoàn thành không thể tạo thêm ghi nhận nữa");
            }

            var record = _mapper.Map<ClassificationRecord>(dto);
            record.ShowQualifiedCount = classification.ShowQualifiedCount - dto.HighQualifiedCount;
            record.StageNumber += (int)classification.Status + 1;
            classification.Status = (ClassificationStatus)record.StageNumber;
            classification.ShowQualifiedCount = record.ShowQualifiedCount;
            classification.HighQualifiedCount = record.HighQualifiedCount;
            classification.Status = ClassificationStatus.Success;
            breed.Status = BreedingStatus.Complete;
            breed.EndDate = DateTime.Now;
            
            if (record.StageNumber == 5)
            {
                throw new Exception("bạn đã hoàn thành quy trình phân ");
            }

            await _recordRepo.CreateAsync(record);
            await _classRepo.UpdateAsync(classification);
            await _breedRepo.UpdateAsync(breed);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClassificationRecordResponseDTO>(record);

        }
        public async Task<bool> DeleteAsync(int id)
        {
            var record = await _recordRepo.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ghi nhận cần xóa");
            }
            await _recordRepo.DeleteAsync(record);

            return await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<ClassificationRecordResponseDTO>> GetAllAsync(ClassificationRecordFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var query = new QueryOptions<ClassificationRecord>
            {
                IncludeProperties = new List<Expression<Func<ClassificationRecord, object>>>
                {
                    p => p.ClassificationStage
                }
            };

            System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>>? predicate = null;
            if (!string.IsNullOrEmpty(filter.Search))
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => (r.Notes != null && r.Notes.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.ClassificationStageId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.ClassificationStageId == filter.ClassificationStageId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinStageNumber.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.StageNumber >= filter.MinStageNumber.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxStageNumber.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.StageNumber <= filter.MaxStageNumber.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
           /* if (filter.MinHighQualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.HighQualifiedCount >= filter.MinHighQualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxHighQualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.HighQualifiedCount <= filter.MaxHighQualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinQualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.QualifiedCount >= filter.MinQualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxQualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.QualifiedCount <= filter.MaxQualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinUnqualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.UnqualifiedCount >= filter.MinUnqualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxUnqualifiedCount.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.UnqualifiedCount <= filter.MaxUnqualifiedCount.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }*/
            if (filter.CreatedFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.CreateAt >= filter.CreatedFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.CreatedTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<ClassificationRecord, bool>> expr = r => r.CreateAt <= filter.CreatedTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            query.Predicate = predicate;

            var fryfish = await _recordRepo.GetAllAsync(query);

            var mappedList = _mapper.Map<List<ClassificationRecordResponseDTO>>(fryfish);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<ClassificationRecordResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<ClassificationRecordResponseDTO?> GetByIdAsync(int id)
        {
            var record = await _recordRepo.GetSingleAsync(new QueryOptions<ClassificationRecord>
            {
                Predicate = e => e.Id == id,
                IncludeProperties = new List<Expression<Func<ClassificationRecord, object>>>
                {
                    p => p.ClassificationStage
                }
            });
            if (record == null)
            {
                throw new KeyNotFoundException(" không tìm thấy ghi nhận");
            }
            return _mapper.Map<ClassificationRecordResponseDTO?>(record);
        }

        public async Task<bool> UpdateAsync(int id, ClassificationRecordUpdateRequestDTO dto)
        {
           
            var record = await _recordRepo.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException(" không tìm thấy ghi nhận");
            }
            _mapper.Map(dto, record);
            await _recordRepo.UpdateAsync(record);
            return await _unitOfWork.SaveAsync();
        }
    }
}
