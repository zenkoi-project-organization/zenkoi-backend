using AutoMapper;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using System.Linq.Expressions;

namespace Zenkoi.BLL.Services.Implements
{
    public class WaterParameterRecordService : IWaterParameterRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<WaterParameterRecord> _recordRepo;
        private readonly IRepoBase<Pond> _pondRepo;
        private readonly IRepoBase<ApplicationUser> _userRepo;

        public WaterParameterRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _recordRepo = _unitOfWork.GetRepo<WaterParameterRecord>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
        }

        public async Task<PaginatedList<WaterParameterRecordResponseDTO>> GetAllAsync(
            WaterParameterRecordFilterDTO? filter, int pageIndex = 1, int pageSize = 10)
        {
            filter ??= new WaterParameterRecordFilterDTO();

            var queryOptions = new QueryOptions<WaterParameterRecord>
            {
                IncludeProperties = new List<Expression<Func<WaterParameterRecord, object>>>
                {
                    r => r.Pond!,
                    r => r.RecordedBy!
                }
            };

            Expression<Func<WaterParameterRecord, bool>>? predicate = null;

            if (filter.PondId.HasValue)
            {
                predicate = predicate.AndAlso(r => r.PondId == filter.PondId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                predicate = predicate.AndAlso(r => r.RecordedAt >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                var toDate = filter.ToDate.Value.AddDays(1).AddSeconds(-1);
                predicate = predicate.AndAlso(r => r.RecordedAt <= toDate);
            }

            if (filter.RecordedByUserId.HasValue)
            {
                predicate = predicate.AndAlso(r => r.RecordedByUserId == filter.RecordedByUserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.NotesContains))
            {
                predicate = predicate.AndAlso(r => r.Notes != null && r.Notes.Contains(filter.NotesContains));
            }

            queryOptions.Predicate = predicate;

            var records = await _recordRepo.GetAllAsync(queryOptions);
            var mapped = _mapper.Map<List<WaterParameterRecordResponseDTO>>(records);

            foreach (var item in mapped)
            {
                item.PondName = (await _pondRepo.GetByIdAsync(item.PondId))?.PondName ?? "Không xác định";
                if (item.RecordedByUserId.HasValue)
                {
                    var user = await _userRepo.GetByIdAsync(item.RecordedByUserId.Value);
                    item.RecordedByUserName = user?.UserName ?? "Không xác định";
                }
            }

            var totalCount = mapped.Count;
            var pagedItems = mapped.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<WaterParameterRecordResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<WaterParameterRecordResponseDTO?> GetByIdAsync(int id)
        {
            var record = await _recordRepo.GetSingleAsync(new QueryOptions<WaterParameterRecord>
            {
                Predicate = r => r.Id == id,
                IncludeProperties = new List<Expression<Func<WaterParameterRecord, object>>>
                {
                    r => r.Pond!,
                    r => r.RecordedBy!
                }
            });

            if (record == null) return null;

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(record);
            result.PondName = record.Pond?.PondName;
            result.RecordedByUserName = record.RecordedBy?.UserName;

            return result;
        }

        public async Task<WaterParameterRecordResponseDTO> CreateAsync(int userId, WaterParameterRecordRequestDTO dto) 
        {
            // Kiểm tra Pond tồn tại
            var pondExists = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pondExists)
                throw new KeyNotFoundException($"Không tìm thấy ao với Id = {dto.PondId}");

           

            var entity = _mapper.Map<WaterParameterRecord>(dto);
            entity.RecordedAt = dto.RecordedAt ?? DateTime.UtcNow;
            entity.RecordedByUserId = userId;

            await _recordRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(entity);
            result.PondName = (await _pondRepo.GetByIdAsync(entity.PondId))?.PondName;
            if (entity.RecordedByUserId.HasValue)
            {
                result.RecordedByUserName = (await _userRepo.GetByIdAsync(entity.RecordedByUserId.Value))?.UserName;
            }

            return result;
        }

        public async Task<WaterParameterRecordResponseDTO?> UpdateAsync(int id, WaterParameterRecordRequestDTO dto)
        {
            var entity = await _recordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            // Kiểm tra Pond
            if (dto.PondId != entity.PondId)
            {
                var pondExists = await _pondRepo.CheckExistAsync(dto.PondId);
                if (!pondExists)
                    throw new KeyNotFoundException($"Không tìm thấy ao với Id = {dto.PondId}");
            }

           

            _mapper.Map(dto, entity);
            if (dto.RecordedAt.HasValue)
                entity.RecordedAt = dto.RecordedAt.Value;

            await _recordRepo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<WaterParameterRecordResponseDTO>(entity);
            result.PondName = (await _pondRepo.GetByIdAsync(entity.PondId))?.PondName;
            result.RecordedByUserName = entity.RecordedByUserId.HasValue
                ? (await _userRepo.GetByIdAsync(entity.RecordedByUserId.Value))?.UserName
                : null;

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _recordRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _recordRepo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}