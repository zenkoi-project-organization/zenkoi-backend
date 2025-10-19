using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ClassificationRecordDTOs;
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

            var record = _mapper.Map<ClassificationRecord>(dto);
            record.StageNumber += (int)classification.Status + 1;
            classification.Status = (ClassificationStatus)record.StageNumber;

            if(record.StageNumber == 4)
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

        public async Task<PaginatedList<ClassificationRecordResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var fryfish = await _recordRepo.GetAllAsync(new QueryOptions<ClassificationRecord>
            {
                IncludeProperties = new List<Expression<Func<ClassificationRecord, object>>>
                {
                    p => p.ClassificationStage
                }
            });

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
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
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
