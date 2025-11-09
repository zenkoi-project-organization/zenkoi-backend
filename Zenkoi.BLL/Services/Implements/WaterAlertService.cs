using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        // 📦 Lấy tất cả cảnh báo nước (phân trang)
        public async Task<PaginatedList<WaterAlertResponseDTO>> GetAllWaterAlertAsync(int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<WaterAlert>
            {
                IncludeProperties = new List<Expression<Func<WaterAlert, object>>>
                {
                    a => a.Pond!,
                    a => a.ResolvedBy!
                },
                OrderBy = q => q.OrderByDescending(a => a.CreatedAt)
            };

            var alerts = await _waterAlertRepo.GetAllAsync(queryOptions);
            var mappedList = _mapper.Map<List<WaterAlertResponseDTO>>(alerts);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<WaterAlertResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<WaterAlertResponseDTO?> GetByIdAsync(int id)
        {
            var alert = await _waterAlertRepo.GetByIdAsync(id);
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
