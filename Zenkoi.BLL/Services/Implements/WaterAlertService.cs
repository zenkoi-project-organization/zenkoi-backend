using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.WaterAlertDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class WaterAlertService : IWaterAlertService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<WaterAlert> _waterAlertRepo;
        private readonly IRepoBase<Pond> _pondRepo;

        public WaterAlertService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _waterAlertRepo = _unitOfWork.GetRepo<WaterAlert>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
        }

        public async Task<PaginatedList<WaterAlertResponseDTO>> GetAllWaterAlertAsync(
            WaterAlertFilterRequestDTO? filter, int pageIndex = 1, int pageSize = 10)
        {
            filter ??= new WaterAlertFilterRequestDTO();

            var queryOptions = new QueryOptions<WaterAlert>
            {
                IncludeProperties = new List<Expression<Func<WaterAlert, object>>>
                {
                    a => a.Pond!,
                }
            };

            Expression<Func<WaterAlert, bool>>? predicate = null;

            if (filter.PondId.HasValue)
                predicate = predicate.AndAlso(a => a.PondId == filter.PondId.Value);

            if (filter.AlertType.HasValue)
                predicate = predicate.AndAlso(a => a.AlertType == filter.AlertType.Value);

            if (filter.Severity.HasValue)
                predicate = predicate.AndAlso(a => a.Severity == filter.Severity.Value);

            if (filter.IsResolved.HasValue)
                predicate = predicate.AndAlso(a => a.IsResolved == filter.IsResolved.Value);

            queryOptions.Predicate = predicate;

            var alerts = await _waterAlertRepo.GetAllAsync(queryOptions);
            var mapped = _mapper.Map<List<WaterAlertResponseDTO>>(alerts);

            foreach (var item in mapped)
            {
                var pond = await _pondRepo.GetByIdAsync(item.PondId);
                item.PondName = pond?.PondName ?? "Không xác định";
            }

            var totalCount = mapped.Count;
            var pagedItems = mapped
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<WaterAlertResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<WaterAlertResponseDTO?> GetByIdAsync(int id)
        {
            var alert = await _waterAlertRepo.GetSingleAsync(new QueryOptions<WaterAlert>
            {
                Predicate = p => p.Id == id , IncludeProperties = new List<Expression<Func<WaterAlert, object>>> { 
                p => p.Pond
                }
            });
            return _mapper.Map<WaterAlertResponseDTO>(alert);
        }

        public async Task<WaterAlertResponseDTO> CreateAsync(int userId, WaterAlertRequestDTO dto)
        {
            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if (pond == null)
                throw new Exception($"Pond with ID {dto.PondId} not found.");

            var alert = new WaterAlert
            {
                PondId = pond.Id,
                ResolvedByUserId = userId,
                Message = $"Thông số nước của ao {pond.PondName} vượt ngưỡng an toàn.",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            };

            await _waterAlertRepo.CreateAsync(alert);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<WaterAlertResponseDTO>(alert);
        }

        public async Task<bool> ResolveAsync(int id, int userId)
        {
            var alert = await _waterAlertRepo.GetByIdAsync(id);
            if (alert == null) return false;

            alert.IsResolved = true;
            alert.ResolvedByUserId = userId;
            alert.ResolveAt = DateTime.UtcNow;

            await _waterAlertRepo.UpdateAsync(alert);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var alert = await _waterAlertRepo.GetByIdAsync(id);
            if (alert == null) return false;

            await _waterAlertRepo.DeleteAsync(alert);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
