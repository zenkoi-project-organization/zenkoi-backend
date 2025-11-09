using AutoMapper;
using Zenkoi.BLL.DTOs.WaterParameterThresholdDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using System.Linq.Expressions;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class WaterParameterThresholdService : IWaterParameterThresholdService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<WaterParameterThreshold> _thresholdRepo;
        private readonly IRepoBase<PondType> _pondTypeRepo;

        public WaterParameterThresholdService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _thresholdRepo = _unitOfWork.GetRepo<WaterParameterThreshold>();
            _pondTypeRepo = _unitOfWork.GetRepo<PondType>();
        }

        public async Task<PaginatedList<WaterParameterThresholdResponseDTO>> GetAllAsync(
        WaterParameterThresholdFilterDTO? filter,
        int pageIndex = 1,
        int pageSize = 10)
        {
            filter ??= new WaterParameterThresholdFilterDTO();

            var queryOptions = new QueryOptions<WaterParameterThreshold>
            {
                IncludeProperties = new List<Expression<Func<WaterParameterThreshold, object>>>
        {
            t => t.PondType
        },
                Tracked = false
            };

            Expression<Func<WaterParameterThreshold, bool>>? predicate = null;

            if (filter.ParameterName.HasValue)
            {
                Expression<Func<WaterParameterThreshold, bool>> expr =
                    t => t.ParameterName.Equals(filter.ParameterName);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.PondTypeId.HasValue)
            {
                Expression<Func<WaterParameterThreshold, bool>> expr =
                    t => t.PondTypeId == filter.PondTypeId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var thresholdsQuery = await _thresholdRepo.GetAllAsync(queryOptions);

            var totalCount = thresholdsQuery.Count();

           var pagedItems = thresholdsQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedList = _mapper.Map<List<WaterParameterThresholdResponseDTO>>(pagedItems);

            return new PaginatedList<WaterParameterThresholdResponseDTO>(
                mappedList,
                totalCount,
                pageIndex,
                pageSize
            );
        }


        public async Task<WaterParameterThresholdResponseDTO?> GetByIdAsync(int id)
        {
            var threshold = await _thresholdRepo.GetSingleAsync(new QueryOptions<WaterParameterThreshold>
            {
                Predicate = t => t.Id == id,
                IncludeProperties = new List<Expression<Func<WaterParameterThreshold, object>>> { t => t.PondType! }
            });

            if (threshold == null) return null;

            var result = _mapper.Map<WaterParameterThresholdResponseDTO>(threshold);
            if (result.PondTypeId.HasValue)
            {
                var pondType = await _pondTypeRepo.GetByIdAsync(result.PondTypeId.Value);
                result.PondTypeName = pondType?.TypeName;
            }

            return result;
        }

        public async Task<WaterParameterThresholdResponseDTO> CreateAsync(WaterParameterThresholdRequestDTO dto)
        {
            // === Validate PondTypeId nếu có ===
            if (dto.PondTypeId.HasValue)
            {
                var exists = await _pondTypeRepo.GetSingleAsync(new QueryOptions<PondType> { 
                Predicate = p => p.Id == dto.PondTypeId
                });

                if (exists == null)
                    throw new KeyNotFoundException($"Không tìm thấy PondType với Id = {dto.PondTypeId.Value}");
            }

            var duplicate = await _thresholdRepo.GetSingleAsync(new QueryOptions<WaterParameterThreshold>
            {
                Predicate = t => t.ParameterName == dto.ParameterName &&
                                 t.PondTypeId == dto.PondTypeId
            });

            if (duplicate != null)
                throw new InvalidOperationException($"Ngưỡng cho thông số '{dto.ParameterName}' đã tồn tại.");

            var entity = _mapper.Map<WaterParameterThreshold>(dto);
            await _thresholdRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<WaterParameterThresholdResponseDTO>(entity);
            if (result.PondTypeId.HasValue)
            {
                var pondType = await _pondTypeRepo.GetByIdAsync(result.PondTypeId.Value);
                result.PondTypeName = pondType?.TypeName;
            }

            return result;
        }
        public async Task<WaterParameterThresholdResponseDTO?> UpdateAsync(int id, WaterParameterThresholdRequestDTO dto)
        {
            var entity = await _thresholdRepo.GetByIdAsync(id);
            if (entity == null) return null;

            // Kiểm tra PondTypeId
            if (dto.PondTypeId.HasValue)
            {
                var exists = await _pondTypeRepo.CheckExistAsync(dto.PondTypeId.Value);
                if (!exists)
                    throw new KeyNotFoundException($"Không tìm thấy PondType với Id = {dto.PondTypeId.Value}");
            }

            // Kiểm tra trùng (trừ chính nó)
            var duplicate = await _thresholdRepo.GetSingleAsync(new QueryOptions<WaterParameterThreshold>
            {
                Predicate = t => t.Id != id &&
                                t.ParameterName == dto.ParameterName &&
                                t.PondTypeId == dto.PondTypeId
            });

            if (duplicate != null)
                throw new InvalidOperationException($"Ngưỡng cho thông số '{dto.ParameterName}' đã tồn tại.");

            _mapper.Map(dto, entity);
            await _thresholdRepo.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<WaterParameterThresholdResponseDTO>(entity);
            if (result.PondTypeId.HasValue)
            {
                var pondType = await _pondTypeRepo.GetByIdAsync(result.PondTypeId.Value);
                result.PondTypeName = pondType?.TypeName;
            }

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _thresholdRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _thresholdRepo.DeleteAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}