using AutoMapper;
using MimeKit.Tnef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.DTOs.WaterParameterRecordDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondService : IPondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Pond> _pondRepo;
        private readonly IWaterParameterRecordService _recordService;
        public PondService(IUnitOfWork unitOfWork, IMapper mapper, IWaterParameterRecordService recordService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _recordService = recordService;
        }
        public async Task<PaginatedList<PondResponseDTO>> GetAllPondsAsync(PondFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Pond>
            {
                IncludeProperties = new List<Expression<Func<Pond, object>>>
        {
            a => a.Area,
            b => b.PondType,
            c => c.WaterParameters,
            d => d.KoiFishes,
            e => e.PondPacketFishes
        }
            };

            var queryBuilder = new QueryBuilder<Pond>()
                .WithTracking(false)
                .WithInclude(p => p.Area)
                .WithInclude(p => p.PondType)
                .WithInclude(p => p.WaterParameters)
                .WithInclude(p => p.KoiFishes)
                .WithInclude(p => p.PondPacketFishes);

            // 👇 List chứa các điều kiện predicate
            var predicates = new List<Expression<Func<Pond, bool>>>();


            // =======================
            // ADD FILTER CONDITIONS
            // =======================
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var keyword = filter.Search.Trim();
                predicates.Add(p =>
                    (p.PondName != null && p.PondName.Contains(keyword)) ||
                    (p.Location != null && p.Location.Contains(keyword)));
            }

            if (filter.Status.HasValue)
                predicates.Add(p => p.PondStatus == filter.Status.Value);

            if (filter.IsNotMaintenance == true)
                predicates.Add(p => p.PondStatus != PondStatus.Maintenance);

            if (filter.AreaId.HasValue)
                predicates.Add(p => p.AreaId == filter.AreaId.Value);

            if (filter.Available == true)
                predicates.Add(p => p.PondStatus == PondStatus.Empty);

            if (filter.PondTypeId.HasValue)
                predicates.Add(p => p.PondTypeId == filter.PondTypeId.Value);

            if (filter.PondTypeEnum.HasValue)
                predicates.Add(p => p.PondType.Type == filter.PondTypeEnum.Value);

            if (filter.MinCapacityLiters.HasValue)
                predicates.Add(p => p.CapacityLiters >= filter.MinCapacityLiters.Value);

            if (filter.MaxCapacityLiters.HasValue)
                predicates.Add(p => p.CapacityLiters <= filter.MaxCapacityLiters.Value);

            if (filter.MinDepthMeters.HasValue)
                predicates.Add(p => p.DepthMeters >= filter.MinDepthMeters.Value);

            if (filter.MaxDepthMeters.HasValue)
                predicates.Add(p => p.DepthMeters <= filter.MaxDepthMeters.Value);

            if (filter.CreatedFrom.HasValue)
                predicates.Add(p => p.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                predicates.Add(p => p.CreatedAt <= filter.CreatedTo.Value);


            // ===========================
            // COMBINE PREDICATES
            // ===========================
            Expression<Func<Pond, bool>>? predicate = null;

            if (predicates.Any())
            {
                predicate = predicates.Aggregate((current, next) =>
                    current.AndAlso(next)); // extension method
            }

            queryOptions.Predicate = predicate;

            // ===========================
            // QUERY DB
            // ===========================
            var ponds = await _pondRepo.GetAllAsync(queryOptions);

            var mappedList = _mapper.Map<List<PondResponseDTO>>(ponds);

            // ===========================
            // UPDATE DTO FIELDS
            // ===========================
            foreach (var pondEntity in ponds)
            {
                var dtoItem = mappedList.First(p => p.Id == pondEntity.Id);

                dtoItem.CurrentCount =
                    (pondEntity.KoiFishes?.Count ?? 0) +
                    (pondEntity.PondPacketFishes?.Sum(x => x.AvailableQuantity) ?? 0);

                var latestRecord = pondEntity.WaterParameters
                    .OrderByDescending(w => w.RecordedAt)
                    .FirstOrDefault();

                dtoItem.record = _mapper.Map<WaterRecordDTO>(latestRecord);
            }

            // ===========================
            // PAGINATION
            // ===========================
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PondResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }



        public async Task<PondResponseDTO?> GetByIdAsync(int id)
        {
            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<Expression<Func<Pond, object>>>
        {
            p => p.Area,
            p => p.PondType,
            p => p.WaterParameters,
            p => p.KoiFishes,          
            p => p.PondPacketFishes    
        }
            });

            if (pond == null) return null;

            var latestWaterParam = pond.WaterParameters
                .OrderByDescending(w => w.RecordedAt)
                .FirstOrDefault();

            var result = _mapper.Map<PondResponseDTO>(pond);
            result.record = _mapper.Map<WaterRecordDTO>(latestWaterParam);

            // 🔥 Auto-update fish count
            result.CurrentCount =
                (pond.KoiFishes?.Count ?? 0)
                + (pond.PondPacketFishes?.Sum(x => x.AvailableQuantity) ?? 0);

            return result;
        }


        public async Task<PondResponseDTO> CreateAsync(int userId, PondRequestDTO dto)
        {
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
                throw new KeyNotFoundException($"không tìm thấy vị trí với AreaId : {dto.AreaId}");

            var pondRepo = _unitOfWork.GetRepo<PondType>();
            var pondType = await pondRepo.GetByIdAsync(dto.PondTypeId);
            if (pondType == null)
                throw new KeyNotFoundException($"không tìm thấy PondTypeId : {dto.PondTypeId}");

            var maxCapacity = dto.DepthMeters * dto.WidthMeters * dto.LengthMeters * 1000;
            if (dto.CurrentCapacity > maxCapacity)
                throw new InvalidOperationException("dung tích thực đang lớn hơn dung tích tối đa của hồ");

            var entity = _mapper.Map<Pond>(dto);
            entity.CapacityLiters = maxCapacity;
            entity.MaxFishCount = pondType.RecommendedQuantity;
            entity.CreatedAt = DateTime.UtcNow;

            await _pondRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var res = _mapper.Map<PondResponseDTO>(entity);

            if (dto.record != null)
            {
                var waterRecordDto = _mapper.Map<WaterParameterRecordRequestDTO>(dto.record);
                waterRecordDto.PondId = entity.Id;

                await _recordService.CreateAsync(userId, waterRecordDto);

                res.record = _mapper.Map<WaterRecordDTO>(waterRecordDto);
            }
            else
            {
                res.record = null;
            }

            return res;
        }

        public async Task<bool> UpdateAsync(int id,int userId ,PondUpdateRequestDTO dto)
        {
            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id  == id, IncludeProperties = new List<Expression<Func<Pond, object>>> { 
                
                    p => p.WaterParameters
                }
            });
            if (pond == null) return false;
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var _pondTypeRepo = _unitOfWork.GetRepo<PondType>();
            var pondType = await _pondTypeRepo.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.PondTypeId}");
            }
            var CapacityLiters = dto.DepthMeters * dto.WidthMeters * dto.LengthMeters * 1000;
            if (dto.CurrentCapacity > CapacityLiters)
            {
                throw new InvalidOperationException("dung tích thực đang lớn hơn dung tích tối đa của hồ");
            }
               var latestWaterParam = pond.WaterParameters
              .OrderByDescending(w => w.RecordedAt)
              .FirstOrDefault();

            if (latestWaterParam != null)
            {
                _mapper.Map(dto, pond);
                pond.CapacityLiters = CapacityLiters;
                var updaterecord = _mapper.Map<WaterParameterRecordRequestDTO>(dto.record);
                updaterecord.PondId = id;
                await _recordService.UpdateAsync(latestWaterParam.Id, updaterecord);
            }
            else
            {
                _mapper.Map(dto, pond);
                pond.CapacityLiters = CapacityLiters;
                var updaterecord = _mapper.Map<WaterParameterRecordRequestDTO>(dto.record);
                updaterecord.PondId = id;
                await _recordService.CreateAsync(userId, updaterecord);

            }
            await _pondRepo.UpdateAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pond = await _pondRepo.GetByIdAsync(id);
            if (pond == null) return false;

            await _pondRepo.DeleteAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<KoiFishResponseDTO>> GetAllKoiFishInPond(int pondId)
        {
            var pondExists = await _pondRepo.CheckExistAsync(pondId);
            if (!pondExists)
            {
                throw new Exception($"Không tìm thấy hồ với Id = {pondId}");
            }

            var koiRepo = _unitOfWork.GetRepo<KoiFish>();

            var queryOptions = new QueryOptions<KoiFish>
            {
                Predicate = k => k.PondId == pondId,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
        {
            k => k.Variety,
            k => k.BreedingProcess
        }
            };
            var koiList = await koiRepo.GetAllAsync(queryOptions);
            var result = _mapper.Map<List<KoiFishResponseDTO>>(koiList);
            return result;
        }
    }
}