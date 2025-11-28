using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PacketFishService : IPacketFishService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<PacketFish> _packetFishRepo;
        private readonly IRepoBase<VarietyPacketFish> _varietyPacketFishRepo;
        private readonly IRepoBase<Variety> _varietyRepo;

        public PacketFishService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
            _varietyPacketFishRepo = _unitOfWork.GetRepo<VarietyPacketFish>();
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
        }

        public async Task<PacketFishResponseDTO> CreatePacketFishAsync(PacketFishRequestDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var packetFishRepo = _unitOfWork.GetRepo<PacketFish>();

                var packetFish = _mapper.Map<PacketFish>(dto);
                packetFish.CreatedAt = DateTime.UtcNow;

                var today = DateTime.UtcNow;
                int months = (today.Year - dto.BirthDate.Year) * 12 + (today.Month - dto.BirthDate.Month);
                if (today.Day < dto.BirthDate.Day)
                    months--;

                packetFish.AgeMonths = Math.Max(months, 0);
                await packetFishRepo.CreateAsync(packetFish);
                await _unitOfWork.SaveChangesAsync();

                foreach (var varietyId in dto.VarietyIds)
                {
                    var variety = await _varietyRepo.GetByIdAsync(varietyId);
                    if (variety == null)
                        throw new ArgumentException($"Variety with ID {varietyId} not found");

                    var varietyPacketFish = new VarietyPacketFish
                    {
                        VarietyId = varietyId,
                        PacketFishId = packetFish.Id
                    };
                    await _varietyPacketFishRepo.CreateAsync(varietyPacketFish);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetPacketFishByIdAsync(packetFish.Id);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<PacketFishResponseDTO> GetPacketFishByIdAsync(int id)
        {
            var packetFish = await _packetFishRepo.GetSingleAsync(new QueryOptions<PacketFish>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<Expression<Func<PacketFish, object>>>
                {
                    p => p.VarietyPacketFishes,
                    p => p.PondPacketFishes
                }
            });

            if (packetFish == null)
                throw new ArgumentException("PacketFish not found");

            if (packetFish.VarietyPacketFishes.Any())
            {
                foreach (var vpf in packetFish.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                            vpf.Variety = variety;
                    }
                }
            }

            var dto = _mapper.Map<PacketFishResponseDTO>(packetFish);
            if (packetFish.PondPacketFishes != null)
            {
                dto.StockQuantity = packetFish.PondPacketFishes
                    .Where(ppf => ppf.IsActive)
                    .Sum(ppf => ppf.AvailableQuantity / packetFish.FishPerPacket);
            }

            dto.Size = packetFish.MinSize + "-" + packetFish.MaxSize;
            return dto;
        }

        public async Task<PaginatedList<PacketFishResponseDTO>> GetAllPacketFishesAsync(PacketFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<PacketFish>()
                .WithTracking(false)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .WithInclude(pf => pf.PondPacketFishes)
                .WithOrderBy(pf => pf.OrderByDescending(x => x.CreatedAt));

            if (!string.IsNullOrEmpty(filter.Search))
            {
                queryBuilder.WithPredicate(pf =>
                    pf.Name.Contains(filter.Search) ||
                    (pf.Description != null && pf.Description.Contains(filter.Search)));
            }

            if (filter.IsAvailable.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.IsAvailable == filter.IsAvailable.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.PricePerPacket >= filter.MinPrice.Value);
            }

            if (filter.MinSize.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.MinSize >= filter.MinSize.Value);
            }

            if (filter.MaxSize.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.MaxSize <= filter.MaxSize.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.PricePerPacket <= filter.MaxPrice.Value);
            }

            if (filter.MinAgeMonths.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.AgeMonths >= filter.MinAgeMonths.Value);
            }

            if (filter.MaxAgeMonths.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.AgeMonths <= filter.MaxAgeMonths.Value);
            }

            if (filter.VarietyIds != null && filter.VarietyIds.Any())
            {
                queryBuilder.WithPredicate(pf =>
                    pf.VarietyPacketFishes.Any(vpf => filter.VarietyIds.Contains(vpf.VarietyId)));
            }

            var query = _packetFishRepo.Get(queryBuilder.Build());
            var paginatedPacketFishes = await PaginatedList<PacketFish>.CreateAsync(query, pageIndex, pageSize);

            foreach (var pf in paginatedPacketFishes)
            {
                foreach (var vpf in pf.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                            vpf.Variety = variety;
                    }
                }
            }

            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(paginatedPacketFishes);

            foreach (var dto in mappedList)
            {
                var packetFish = paginatedPacketFishes.FirstOrDefault(pf => pf.Id == dto.Id);
                if (packetFish != null)
                {
                    if (packetFish.PondPacketFishes != null)
                    {
                        dto.StockQuantity = packetFish.PondPacketFishes
                            .Where(ppf => ppf.IsActive)
                            .Sum(ppf => ppf.AvailableQuantity / packetFish.FishPerPacket);
                    }
                    dto.Size = packetFish.MinSize + "-" + packetFish.MaxSize;
                }
            }

            return new PaginatedList<PacketFishResponseDTO>(
                mappedList,
                paginatedPacketFishes.TotalItems,
                paginatedPacketFishes.PageIndex,
                pageSize);
        }

        public async Task<PacketFishResponseDTO> UpdatePacketFishAsync(int id, PacketFishUpdateDTO packetFishUpdateDTO)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var packetFish = await _packetFishRepo.GetByIdAsync(id);
                if (packetFish == null)
                    throw new ArgumentException("PacketFish not found");

                _mapper.Map(packetFishUpdateDTO, packetFish);
                packetFish.UpdatedAt = DateTime.UtcNow;

                await _packetFishRepo.UpdateAsync(packetFish);

                var existingVarieties = await _varietyPacketFishRepo.GetAllAsync(new QueryOptions<VarietyPacketFish>
                {
                    Predicate = vpf => vpf.PacketFishId == id
                });

                foreach (var existingVariety in existingVarieties)
                    await _varietyPacketFishRepo.DeleteAsync(existingVariety);

                foreach (var varietyId in packetFishUpdateDTO.VarietyIds)
                {
                    var variety = await _varietyRepo.GetByIdAsync(varietyId);
                    if (variety == null)
                        throw new ArgumentException($"Variety with ID {varietyId} not found");

                    var varietyPacketFish = new VarietyPacketFish
                    {
                        VarietyId = varietyId,
                        PacketFishId = id
                    };
                    await _varietyPacketFishRepo.CreateAsync(varietyPacketFish);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return await GetPacketFishByIdAsync(id);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<bool> DeletePacketFishAsync(int id)
        {
            var packetFish = await _packetFishRepo.GetByIdAsync(id);
            if (packetFish == null)
                return false;

            await _packetFishRepo.DeleteAsync(packetFish);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedList<PacketFishResponseDTO>> GetPacketFishesBySizeAsync(
            double? minSize, double? maxSize, int pageIndex = 1, int pageSize = 10)
        {
            var queryBuilder = new QueryBuilder<PacketFish>()
                .WithTracking(false)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .WithOrderBy(pf => pf.OrderByDescending(x => x.CreatedAt));

            queryBuilder.WithPredicate(pf => pf.IsAvailable == true);

            if (minSize.HasValue && maxSize.HasValue)
            {
                queryBuilder.WithPredicate(pf =>
                    pf.MinSize <= maxSize.Value &&
                    pf.MaxSize >= minSize.Value);
            }
            else if (minSize.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.MaxSize >= minSize.Value);
            }
            else if (maxSize.HasValue)
            {
                queryBuilder.WithPredicate(pf => pf.MinSize <= maxSize.Value);
            }

            var packetFishes = await _packetFishRepo.GetAllAsync(queryBuilder.Build());

            foreach (var pf in packetFishes)
            {
                foreach (var vpf in pf.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                            vpf.Variety = variety;
                    }
                }
            }

          
            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);

            foreach (var dto in mappedList)
            {
                var pf = packetFishes.FirstOrDefault(x => x.Id == dto.Id);
                if (pf != null)
                    dto.Size = pf.MinSize + "-" + pf.MaxSize;
            }

 
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PaginatedList<PacketFishResponseDTO>> GetPacketFishesByPriceRangeAsync(decimal minPrice, decimal maxPrice, int pageIndex = 1, int pageSize = 10)
        {
            var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                .WithPredicate(pf => pf.PricePerPacket >= minPrice && pf.PricePerPacket <= maxPrice && pf.IsAvailable == true)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .WithOrderBy(pf => pf.OrderBy(x => x.PricePerPacket))
                .Build());

            foreach (var pf in packetFishes)
            {
                foreach (var vpf in pf.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                            vpf.Variety = variety;
                    }
                }
            }

            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);

            foreach (var dto in mappedList)
            {
                var packetFish = packetFishes.FirstOrDefault(pf => pf.Id == dto.Id);
                if (packetFish != null)
                    dto.Size = packetFish.MinSize + "-" + packetFish.MaxSize;
            }

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PaginatedList<PacketFishResponseDTO>> GetAvailablePacketFishesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                 .WithPredicate(pf => pf.IsAvailable == true)
                 .WithInclude(pf => pf.VarietyPacketFishes)
                 .WithOrderBy(pf => pf.OrderByDescending(x => x.CreatedAt))
                 .Build());

            foreach (var pf in packetFishes)
            {
                foreach (var vpf in pf.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                            vpf.Variety = variety;
                    }
                }
            }

            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);

            foreach (var dto in mappedList)
            {
                var packetFish = packetFishes.FirstOrDefault(pf => pf.Id == dto.Id);
                if (packetFish != null)
                    dto.Size = packetFish.MinSize + "-" + packetFish.MaxSize;
            }

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }
    }
}
