using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
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
        public PondService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pondRepo = _unitOfWork.GetRepo<Pond>();
        }
        public async Task<PaginatedList<PondResponseDTO>> GetAllPondsAsync(PondFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Pond>
            {
                IncludeProperties = new List<Expression<Func<Pond, object>>>
                {
                    a => a.Area,
                    b => b.PondType
                }
            };

            // Apply filters using PredicateBuilder
            System.Linq.Expressions.Expression<System.Func<Pond, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.PondName.Contains(filter.Search) || p.Location.Contains(filter.Search);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Status.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.PondStatus == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.AreaId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.AreaId == filter.AreaId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.PondTypeId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.PondTypeId == filter.PondTypeId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinCapacityLiters.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.CapacityLiters >= filter.MinCapacityLiters.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxCapacityLiters.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.CapacityLiters <= filter.MaxCapacityLiters.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinDepthMeters.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.DepthMeters >= filter.MinDepthMeters.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxDepthMeters.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.DepthMeters <= filter.MaxDepthMeters.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.CreatedFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.CreatedAt >= filter.CreatedFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.CreatedTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<Pond, bool>> expr = p => p.CreatedAt <= filter.CreatedTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var ponds = await _pondRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<PondResponseDTO>>(ponds);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PondResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }


        public async Task<PondResponseDTO?> GetByIdAsync(int id)
        {
            var pondtypes = await _pondRepo.GetByIdAsync(id);
            return _mapper.Map<PondResponseDTO>(pondtypes);
        }

        public async Task<PondResponseDTO> CreateAsync(PondRequestDTO dto)
        {
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var pondRepo = _unitOfWork.GetRepo<PondType>();
            var pondType = await pondRepo.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.PondTypeId}");
            }

            var entity = _mapper.Map<Pond>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _pondRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PondRequestDTO dto)
        {
            var pond = await _pondRepo.GetByIdAsync(id);
            if (pond == null) return false;
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var pondRepo = _unitOfWork.GetRepo<Pond>();
            var pondType = await pondRepo.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.PondTypeId}");
            }

            _mapper.Map(dto, pond);
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
    }
}