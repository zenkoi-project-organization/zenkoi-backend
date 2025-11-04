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

                await packetFishRepo.CreateAsync(packetFish);
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
                Predicate = p => p.Id == id , IncludeProperties = new List<Expression<Func<PacketFish, object>>> {
                p => p.VarietyPacketFishes
                }
            }); ;


            if (packetFish != null && packetFish.VarietyPacketFishes.Any())
            {
                foreach (var vpf in packetFish.VarietyPacketFishes)
                {
                    if (vpf.VarietyId > 0)
                    {
                        var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                        if (variety != null)
                        {
                            vpf.Variety = variety;
                        }
                    }
                }
            }

            if (packetFish == null)
            {
                throw new ArgumentException("PacketFish not found");
            }

            return _mapper.Map<PacketFishResponseDTO>(packetFish);
        }

        public async Task<PaginatedList<PacketFishResponseDTO>> GetAllPacketFishesAsync(PacketFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<PacketFish>
            {
                IncludeProperties = new List<Expression<Func<PacketFish, object>>>
                {
                    pf => pf.VarietyPacketFishes
                }
            };

            Expression<Func<PacketFish, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                Expression<Func<PacketFish, bool>> expr = pf => 
                    pf.Name.Contains(filter.Search) || 
                    (pf.Description != null && pf.Description.Contains(filter.Search));
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.IsAvailable.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.IsAvailable == filter.IsAvailable.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Size.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.Size == filter.Size.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinPrice.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.TotalPrice >= filter.MinPrice.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxPrice.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.TotalPrice <= filter.MaxPrice.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinAgeMonths.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.AgeMonths >= filter.MinAgeMonths.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxAgeMonths.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.AgeMonths <= filter.MaxAgeMonths.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MinQuantity.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.Quantity >= filter.MinQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.MaxQuantity.HasValue)
            {
                Expression<Func<PacketFish, bool>> expr = pf => pf.Quantity <= filter.MaxQuantity.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;
            queryOptions.OrderBy = pf => pf.OrderByDescending(x => x.CreatedAt);

            var packetFishes = await _packetFishRepo.GetAllAsync(queryOptions);

            foreach (var pf in packetFishes)
            {
                if (pf.VarietyPacketFishes.Any())
                {
                    foreach (var vpf in pf.VarietyPacketFishes)
                    {
                        if (vpf.VarietyId > 0)
                        {
                            var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                            if (variety != null)
                            {
                                vpf.Variety = variety;
                            }
                        }
                    }
                }
            }

            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PacketFishResponseDTO> UpdatePacketFishAsync(int id, PacketFishUpdateDTO packetFishUpdateDTO)
        {
            var packetFish = await _packetFishRepo.GetByIdAsync(id);
            if (packetFish == null)
            {
                throw new ArgumentException("PacketFish not found");
            }

            _mapper.Map(packetFishUpdateDTO, packetFish);
            packetFish.UpdatedAt = DateTime.UtcNow;

            await _packetFishRepo.UpdateAsync(packetFish);
            await _unitOfWork.SaveChangesAsync();

            return await GetPacketFishByIdAsync(id);
        }

        public async Task<bool> DeletePacketFishAsync(int id)
        {
            var packetFish = await _packetFishRepo.GetByIdAsync(id);
            if (packetFish == null)
            {
                return false;
            }

            await _packetFishRepo.DeleteAsync(packetFish);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

       
        public async Task<PaginatedList<PacketFishResponseDTO>> GetPacketFishesBySizeAsync(FishSize size ,int pageIndex = 1, int pageSize = 10)
        {
            var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                .WithPredicate(pf => pf.Size == size && pf.IsAvailable == true)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .WithOrderBy(pf => pf.OrderByDescending(x => x.CreatedAt))
                .Build());

            foreach (var pf in packetFishes)
            {
                if (pf.VarietyPacketFishes.Any())
                {
                    foreach (var vpf in pf.VarietyPacketFishes)
                    {
                        if (vpf.VarietyId > 0)
                        {
                            var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                            if (variety != null)
                            {
                                vpf.Variety = variety;
                            }
                        }
                    }
                }
            }
  


            foreach (var pf in packetFishes)
            {
                if (pf.VarietyPacketFishes.Any())
                {
                    foreach (var vpf in pf.VarietyPacketFishes)
                    {
                        if (vpf.VarietyId > 0)
                        {
                            var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                            if (variety != null)
                            {
                                vpf.Variety = variety;
                            }
                        }
                    }
                }
            }
            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);
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
                .WithPredicate(pf => pf.TotalPrice >= minPrice && pf.TotalPrice <= maxPrice && pf.IsAvailable == true)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .WithOrderBy(pf => pf.OrderBy(x => x.TotalPrice))
                .Build());

            foreach (var pf in packetFishes)
            {
                if (pf.VarietyPacketFishes.Any())
                {
                    foreach (var vpf in pf.VarietyPacketFishes)
                    {
                        if (vpf.VarietyId > 0)
                        {
                            var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                            if (variety != null)
                            {
                                vpf.Variety = variety;
                            }
                        }
                    }
                }
            }
            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);

        }

        public async  Task<PaginatedList<PacketFishResponseDTO>> GetAvailablePacketFishesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
                 .WithPredicate(pf => pf.IsAvailable == true)
                 .WithInclude(pf => pf.VarietyPacketFishes)
                 .WithOrderBy(pf => pf.OrderByDescending(x => x.CreatedAt))
                 .Build());


            foreach (var pf in packetFishes)
            {
                if (pf.VarietyPacketFishes.Any())
                {
                    foreach (var vpf in pf.VarietyPacketFishes)
                    {
                        if (vpf.VarietyId > 0)
                        {
                            var variety = await _varietyRepo.GetByIdAsync(vpf.VarietyId);
                            if (variety != null)
                            {
                                vpf.Variety = variety;
                            }
                        }
                    }
                }
            }
            var mappedList = _mapper.Map<List<PacketFishResponseDTO>>(packetFishes);
            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return new PaginatedList<PacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }
    }
}
