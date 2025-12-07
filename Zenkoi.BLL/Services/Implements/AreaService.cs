using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using System.Linq.Expressions;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Area> _areaRepo;
        public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _areaRepo = _unitOfWork.GetRepo<Area>();
        }
        public async Task<PaginatedList<AreaResponseDTO>> GetAllAsync(AreaFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Area>();

            Expression<Func<Area, bool>>? predicate = a => !a.IsDeleted;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<Area, bool>> expr = a => a.AreaName.Contains(filter.Search) || (a.Description != null && a.Description.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinTotalAreaSQM.HasValue)
            {
                Expression<Func<Area, bool>> expr = a => a.TotalAreaSQM >= filter.MinTotalAreaSQM.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxTotalAreaSQM.HasValue)
            {
                Expression<Func<Area, bool>> expr = a => a.TotalAreaSQM <= filter.MaxTotalAreaSQM.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var areas = await _areaRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<AreaResponseDTO>>(areas);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<AreaResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<AreaResponseDTO?> GetByIdAsync(int id)
        {
            var area = await _areaRepo.GetByIdAsync(id);
            if (area == null)
            {
                throw new KeyNotFoundException("không tìm thấy ví trí");
            }
            return _mapper.Map<AreaResponseDTO>(area);
        }

        public async Task<AreaResponseDTO> CreateAsync(AreaRequestDTO dto)
        {

            var entity = _mapper.Map<Area>(dto);
            await _areaRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AreaResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, AreaRequestDTO dto)
        {
            var area = await _areaRepo.GetByIdAsync(id);
            if (area == null) if (area == null)
                {
                    throw new KeyNotFoundException("không tìm thấy ví trí");
                }
            _mapper.Map(dto, area);
            await _areaRepo.UpdateAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _areaRepo.GetSingleAsync(new QueryOptions<Area>
            {
                Predicate = area => area.Id == id,
                IncludeProperties = new List<Expression<Func<Area, object>>> {
                p => p.Ponds
                }
            });
            if (area == null)
            {
                throw new KeyNotFoundException("Không tìm thấy vị trí");
            }

            if (area.Ponds != null && area.Ponds.Any(p => !p.IsDeleted))
            {
                throw new InvalidOperationException("Không thể xóa Area vì đang có hồ thuộc khu vực này.");
            }

            area.IsDeleted = true;
            area.DeletedAt = DateTime.UtcNow;
            area.UpdatedAt = DateTime.UtcNow;

            await _areaRepo.UpdateAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}