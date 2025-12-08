using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.IncidentTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class IncidentTypeService : IIncidentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<IncidentType> _incidentTypeRepo;

        public IncidentTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _incidentTypeRepo = _unitOfWork.GetRepo<IncidentType>();
        }

        public async Task<PaginatedList<IncidentTypeResponseDTO>> GetAllIncidentTypesAsync(
            IncidentTypeFilterRequestDTO filter,
            int pageIndex = 1,
            int pageSize = 10)
        {
            filter ??= new IncidentTypeFilterRequestDTO();

            var queryBuilder = new QueryBuilder<IncidentType>()
                .WithPredicate(it => !it.IsDeleted)
                .WithTracking(false)
                .WithOrderBy(q => q.OrderByDescending(it => it.Id));

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var keyword = filter.Search.Trim();
                queryBuilder.WithPredicate(it =>
                    it.Name.Contains(keyword) ||
                    (it.Description != null && it.Description.Contains(keyword)));
            }

            if (filter.AffectsBreeding.HasValue)
            {
                queryBuilder.WithPredicate(it => it.AffectsBreeding == filter.AffectsBreeding.Value);
            }

            var query = _incidentTypeRepo.Get(queryBuilder.Build());
            var paginatedEntities = await PaginatedList<IncidentType>.CreateAsync(query, pageIndex, pageSize);
            var resultDto = _mapper.Map<List<IncidentTypeResponseDTO>>(paginatedEntities);

            return new PaginatedList<IncidentTypeResponseDTO>(
                resultDto,
                paginatedEntities.TotalItems,
                pageIndex,
                pageSize);
        }

        public async Task<IncidentTypeResponseDTO> GetIncidentTypeByIdAsync(int id)
        {
            var incidentType = await _incidentTypeRepo.GetSingleAsync(new QueryBuilder<IncidentType>()
                .WithPredicate(it => it.Id == id)
                .WithTracking(false)
                .Build());

            if (incidentType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy loại sự cố với id {id}.");
            }

            return _mapper.Map<IncidentTypeResponseDTO>(incidentType);
        }

        public async Task<IncidentTypeResponseDTO> CreateIncidentTypeAsync(IncidentTypeRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            var checkQuery = new QueryBuilder<IncidentType>()
                        .WithPredicate(it => it.Name == dto.Name)
                        .WithTracking(false)
                        .Build();
          
            var existingType = await _incidentTypeRepo.GetSingleAsync(checkQuery);
            if (existingType != null)
            {
                throw new InvalidOperationException($"Loại sự cố với tên '{dto.Name}' đã tồn tại.");
            }
            var incidentType = _mapper.Map<IncidentType>(dto);

            await _incidentTypeRepo.CreateAsync(incidentType);
            await _unitOfWork.SaveChangesAsync();

            return await GetIncidentTypeByIdAsync(incidentType.Id);
        }

        public async Task<IncidentTypeResponseDTO> UpdateIncidentTypeAsync(int id, IncidentTypeUpdateRequestDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var incidentType = await _incidentTypeRepo.GetByIdAsync(id);
            if (incidentType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy loại sự cố với id {id}.");
            }

            if (dto.Name != incidentType.Name)
            {
                var checkQuery = new QueryBuilder<IncidentType>()
                                .WithPredicate(it => it.Name == dto.Name)
                                .WithTracking(false)
                                .Build();

                var duplicateType = await _incidentTypeRepo.GetSingleAsync(checkQuery);
                if (duplicateType != null)
                {
                    throw new InvalidOperationException($"Tên loại sự cố '{dto.Name}' đã được sử dụng bởi một mục khác.");
                }
            }
            _mapper.Map(dto, incidentType);

            await _incidentTypeRepo.UpdateAsync(incidentType);
            await _unitOfWork.SaveChangesAsync();

            return await GetIncidentTypeByIdAsync(id);
        }

        public async Task<bool> DeleteIncidentTypeAsync(int id)
        {
            var incidentType = await _incidentTypeRepo.GetByIdAsync(id);
            if (incidentType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy loại sự cố với id {id}.");
            }

            await _incidentTypeRepo.DeleteAsync(incidentType);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}

