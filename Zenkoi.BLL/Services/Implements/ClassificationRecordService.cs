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
        public ClassificationRecordService(IUnitOfWork unitOfWork, IMapper mapper)
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
            if (classification == null)
                throw new KeyNotFoundException("Không tìm thấy bầy phân loại");

            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status == ClassificationStatus.Success)
                throw new InvalidOperationException("Phân loại đã hoàn tất, không thể tạo thêm ghi nhận mới");

            var record = _mapper.Map<ClassificationRecord>(dto);

            var existingRecords = await _recordRepo.GetAllAsync(new QueryOptions<ClassificationRecord>
            {
                Predicate = r => r.ClassificationStageId == dto.ClassificationStageId,
                OrderBy = q => q.OrderBy(r => r.StageNumber),
                Tracked = false
            });

            // ✅ Validate cơ bản
            if (dto.CullQualifiedCount < 0)
                throw new InvalidOperationException("Số cá loại bỏ không hợp lệ.");

            if (!existingRecords.Any())
            {
                if (dto.CullQualifiedCount >= classification.TotalCount)
                    throw new InvalidOperationException("Số cá loại bỏ vượt quá tổng số cá ban đầu.");

                record.StageNumber = 1;
                record.PondQualifiedCount = classification.TotalCount - dto.CullQualifiedCount;

                classification.Status = (ClassificationStatus)record.StageNumber;
                classification.PondQualifiedCount = record.PondQualifiedCount;
                classification.CullQualifiedCount = record.CullQualifiedCount;
            }
            else
            {
                var lastRecord = existingRecords.Last();

                if (dto.CullQualifiedCount > lastRecord.PondQualifiedCount)
                    throw new InvalidOperationException("Số cá loại bỏ vượt quá số cá hiện có trong hồ.");

                record.StageNumber = lastRecord.StageNumber + 1;
                record.PondQualifiedCount = lastRecord.PondQualifiedCount - dto.CullQualifiedCount;

                classification.Status = (ClassificationStatus)record.StageNumber;
                classification.PondQualifiedCount = record.PondQualifiedCount;
                classification.CullQualifiedCount += record.CullQualifiedCount;
            }  

            if (record.StageNumber == 4)
            {
                classification.Status = ClassificationStatus.Success;
                breed.Status = BreedingStatus.Complete;
                breed.EndDate = DateTime.Now;
            }

            if (record.StageNumber > 4)
                throw new InvalidOperationException("Bạn đã hoàn thành quy trình phân loại.");

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
                throw new KeyNotFoundException("Không tìm thấy bầy phân loại");

            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status == ClassificationStatus.Success)
                throw new InvalidOperationException("Phân loại đã hoàn tất, không thể tạo thêm ghi nhận mới");

            var record = _mapper.Map<ClassificationRecord>(dto);

            // ✅ Validate dữ liệu nhập
            if (dto.HighQualifiedCount < 0)
                throw new InvalidOperationException("Số cá High không hợp lệ.");

            if (dto.HighQualifiedCount > classification.PondQualifiedCount)
                throw new InvalidOperationException("Số cá High vượt quá số cá hiện có trong hồ.");

            record.PondQualifiedCount = classification.PondQualifiedCount - dto.HighQualifiedCount;
            record.StageNumber = (int)classification.Status + 1;

            classification.Status = (ClassificationStatus)record.StageNumber;
            classification.HighQualifiedCount = record.HighQualifiedCount;
            classification.PondQualifiedCount = record.PondQualifiedCount;
            
            if (record.StageNumber == 4)
            {
                classification.Status = ClassificationStatus.Success;
                breed.Status = BreedingStatus.Complete;
                breed.EndDate = DateTime.Now;
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
                throw new KeyNotFoundException("Không tìm thấy bầy phân loại");

            var breed = await _breedRepo.GetByIdAsync(classification.BreedingProcessId);
            if (classification.Status == ClassificationStatus.Success)
                throw new InvalidOperationException("Phân loại đã hoàn tất, không thể tạo thêm ghi nhận mới");

            var record = _mapper.Map<ClassificationRecord>(dto);

            // ✅ Validate dữ liệu nhập
            if (dto.ShowQualifiedCount < 0)
                throw new InvalidOperationException("Số cá Show không hợp lệ.");

            if (dto.ShowQualifiedCount > classification.HighQualifiedCount)
                throw new InvalidOperationException("Số cá Show vượt quá số cá High hiện có.");

            record.HighQualifiedCount = classification.HighQualifiedCount - dto.ShowQualifiedCount;
            record.StageNumber = (int)classification.Status + 1;

            classification.Status = (ClassificationStatus)record.StageNumber;
            classification.ShowQualifiedCount = dto.ShowQualifiedCount;
            classification.HighQualifiedCount = record.HighQualifiedCount;

            // ✅ Hoàn tất quy trình
            classification.Status = ClassificationStatus.Success;
            breed.Status = BreedingStatus.Complete;
            breed.EndDate = DateTime.Now;

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

        public async Task<ClassificationSummaryDTO> GetSummaryAsync(int classificationStageId)
        {
            var _classRepo = _unitOfWork.GetRepo<ClassificationStage>();
            var _recordRepo = _unitOfWork.GetRepo<ClassificationRecord>();


            var classification = await _classRepo.GetByIdAsync(classificationStageId);
            if (classification == null)
                throw new KeyNotFoundException("Không tìm thấy bầy phân loại");

            var records = await _recordRepo.GetAllAsync(new QueryOptions<ClassificationRecord>
            {
                Predicate = r => r.ClassificationStageId == classificationStageId,
                OrderBy = q => q.OrderBy(r => r.StageNumber),
                Tracked = false
            });

            if (!records.Any())
                throw new InvalidOperationException("Chưa có ghi nhận phân loại nào");

            var stage1 = records.FirstOrDefault(r => r.StageNumber == 1);
            var stage2 = records.FirstOrDefault(r => r.StageNumber == 2);
            var stage3 = records.FirstOrDefault(r => r.StageNumber == 3);
            var stage4 = records.FirstOrDefault(r => r.StageNumber == 4);

            int cull = 0, pond = classification.TotalCount, high = 0, show = 0;

            if (stage1 != null)
            {
                int cull1 = stage1.CullQualifiedCount ?? 0;
                cull += cull1;
                pond -= cull1;
            }


            if (stage2 != null)
            {
                int cull2 = stage2.CullQualifiedCount ?? 0;
                cull += cull2;
                pond -= cull2;


                if (stage3 != null)
                {
                    int high3 = stage3.HighQualifiedCount ?? 0;
                    high = high3;
                    pond -= high3;
                }

                if (stage4 != null)
                {
                    int show4 = stage4.ShowQualifiedCount ?? 0;
                    show = show4;


                    high = (stage3?.HighQualifiedCount ?? high) - show4;
                }

                var currentStage = records.Max(r => r.StageNumber);
                bool isCompleted = currentStage >= 4;
            }
            return new ClassificationSummaryDTO
            {
                ClassificationStageId = classificationStageId,
                TotalCullQualified = cull,
                TotalPondQualified = pond,
                TotalHighQualified = high,
                TotalShowQualified = show,
                CurrentFish = pond + high + show,
            };
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
