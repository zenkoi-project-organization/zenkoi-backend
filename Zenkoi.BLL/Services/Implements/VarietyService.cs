using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using System.Linq.Expressions;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class VarietyService : IVarietyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Variety> _varietyRepo;
        public VarietyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
        }
        public async Task<PaginatedList<VarietyResponseDTO>> GetAllVarietiesAsync(VarietyFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Variety>();

            Expression<Func<Variety, bool>>? predicate = v => !v.IsDeleted;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<Variety, bool>> expr = v => v.VarietyName.Contains(filter.Search) || v.Characteristic.Contains(filter.Search) || v.OriginCountry.Contains(filter.Search);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (!string.IsNullOrEmpty(filter.OriginCountry))
            {
                Expression<Func<Variety, bool>> expr = v => v.OriginCountry.Contains(filter.OriginCountry);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var varieties = await _varietyRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<VarietyResponseDTO>>(varieties);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<VarietyResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<VarietyResponseDTO?> GetByIdAsync(int id)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            return _mapper.Map<VarietyResponseDTO>(variety);
        }

        public async Task<VarietyResponseDTO> CreateAsync(VarietyRequestDTO dto)
        {
          

            var entity = _mapper.Map<Variety>(dto);
           
            await _varietyRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<VarietyResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, VarietyRequestDTO dto)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            if (variety == null) return false;

            _mapper.Map(dto, variety);
            await _varietyRepo.UpdateAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            if (variety == null) return false;

            await _varietyRepo.DeleteAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}