using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PromotionDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Promotion> _promotionRepo;

        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _promotionRepo = _unitOfWork.GetRepo<Promotion>();
        }

        public async Task<PaginatedList<PromotionResponseDTO>> GetAllAsync(PromotionFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Promotion>();

            Expression<Func<Promotion, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<Promotion, bool>> expr = p => p.Code.Contains(filter.Search) || (p.Description != null && p.Description.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.IsActive.HasValue)
            {
                Expression<Func<Promotion, bool>> expr = p => p.IsActive == filter.IsActive.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.DiscountType.HasValue)
            {
                Expression<Func<Promotion, bool>> expr = p => p.DiscountType == filter.DiscountType.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.AvailableOnDate.HasValue)
            {
                var date = filter.AvailableOnDate.Value.Date;
                Expression<Func<Promotion, bool>> expr = p => p.ValidFrom.Date <= date && p.ValidTo.Date >= date;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var promotions = await _promotionRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<PromotionResponseDTO>>(promotions);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PromotionResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }
        public async Task<PromotionResponseDTO?> GetByIdAsync(int id)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            return _mapper.Map<PromotionResponseDTO>(promotion);
        }
        public async Task<PromotionResponseDTO> CreateAsync(PromotionRequestDTO dto)
        {         
            var existingCode = await _promotionRepo.AnyAsync(new QueryOptions<Promotion>
            {
                Predicate = p => p.Code == dto.Code
            });
            if (existingCode)
            {
                throw new ArgumentException($"Mã khuyến mãi '{dto.Code}' đã tồn tại.");
            }
            if (dto.ValidFrom >= dto.ValidTo)
            {
                throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
            }

            var entity = _mapper.Map<Promotion>(dto);
            await _promotionRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PromotionResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PromotionRequestDTO dto)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            if (promotion == null) return false;
        
            var existingCode = await _promotionRepo.AnyAsync(new QueryOptions<Promotion>
            {
                Predicate = p => p.Code == dto.Code && p.Id != id
            });
            if (existingCode)
            {
                throw new ArgumentException($"Mã khuyến mãi '{dto.Code}' đã tồn tại.");
            }
            if (dto.ValidFrom >= dto.ValidTo)
            {
                throw new ArgumentException("Ngày bắt đầu phải trước ngày kết thúc.");
            }

            _mapper.Map(dto, promotion);
            await _promotionRepo.UpdateAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            if (promotion == null) return false;

    
            promotion.IsDeleted = true;
            await _promotionRepo.UpdateAsync(promotion);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
