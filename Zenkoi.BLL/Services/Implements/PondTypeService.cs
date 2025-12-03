using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondTypeService : IPondTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<PondType> _pondtypeRepo;
        public PondTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pondtypeRepo = _unitOfWork.GetRepo<PondType>();
        }
        public async Task<PaginatedList<PondTypeResponseDTO>> GetAllAsync(PondTypeFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<PondType>();

            System.Linq.Expressions.Expression<System.Func<PondType, bool>>? predicate = pt => !pt.IsDeleted;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                System.Linq.Expressions.Expression<System.Func<PondType, bool>> expr = pt => pt.TypeName.Contains(filter.Search) || (pt.Description != null && pt.Description.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.Type.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<PondType, bool>> expr =
                   p => p.Type == filter.Type.Value;

                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinRecommendedQuantity.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<PondType, bool>> expr = pt => pt.RecommendedQuantity >= filter.MinRecommendedQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxRecommendedQuantity.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<PondType, bool>> expr = pt => pt.RecommendedQuantity <= filter.MaxRecommendedQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var pondTypes = await _pondtypeRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<PondTypeResponseDTO>>(pondTypes);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PondTypeResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PondTypeResponseDTO?> GetByIdAsync(int id)
        {
            var pondtypes = await _pondtypeRepo.GetByIdAsync(id);
            return _mapper.Map<PondTypeResponseDTO>(pondtypes);
        }

        public async Task<PondTypeResponseDTO> CreateAsync(PondTypeRequestDTO dto)
        {

            var existsOptions = new QueryOptions<PondType>
            {
                Predicate = p => p.TypeName.ToLower() == dto.TypeName.ToLower()
                                 && !p.IsDeleted
            };

            bool exists = await _pondtypeRepo.AnyAsync(existsOptions);

            if (exists)
                throw new InvalidOperationException($"TypeName '{dto.TypeName}' đã được tạo.");

            var entity = _mapper.Map<PondType>(dto);
            await _pondtypeRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondTypeResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PondTypeRequestDTO dto)
        {
            var pondtype = await _pondtypeRepo.GetByIdAsync(id);
            if (pondtype == null) return false;

            _mapper.Map(dto, pondtype);
            await _pondtypeRepo.UpdateAsync(pondtype);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
        
            var pondType = await _pondtypeRepo.GetSingleAsync(new QueryOptions<PondType>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<PondType, object>>> {
                p => p.Ponds
                }
            });
            if (pondType == null)
                return false;

            if (pondType.Ponds != null && pondType.Ponds.Any(p => !p.IsDeleted))
            {
                throw new InvalidOperationException("Không thể xoá loại hồ vì đang có hồ sử dụng loại này.");
            }

            pondType.IsDeleted = true;
            pondType.DeletedAt = DateTime.Now;

            await _pondtypeRepo.UpdateAsync(pondType);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
  }