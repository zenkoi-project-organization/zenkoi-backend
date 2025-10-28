using AutoMapper;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.PacketFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
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
            var packetFish = await _packetFishRepo.GetSingleAsync(new QueryBuilder<PacketFish>()
                .WithPredicate(pf => pf.Id == id)
                .WithInclude(pf => pf.VarietyPacketFishes)
                .Build());

     
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

        public async Task<IEnumerable<PacketFishResponseDTO>> GetAllPacketFishesAsync(QueryOptions<PacketFish>? queryOptions = null)
        {
            if (queryOptions == null)
            {
                var packetFishes = await _packetFishRepo.GetAllAsync(new QueryBuilder<PacketFish>()
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

                return _mapper.Map<IEnumerable<PacketFishResponseDTO>>(packetFishes);
            }

            var packetFishesWithCustomOptions = await _packetFishRepo.GetAllAsync(queryOptions);
            return _mapper.Map<IEnumerable<PacketFishResponseDTO>>(packetFishesWithCustomOptions);
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

        public async Task<IEnumerable<PacketFishResponseDTO>> GetAvailablePacketFishesAsync()
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

            return _mapper.Map<IEnumerable<PacketFishResponseDTO>>(packetFishes);
        }

        public async Task<IEnumerable<PacketFishResponseDTO>> GetPacketFishesBySizeAsync(FishSize size)
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

            return _mapper.Map<IEnumerable<PacketFishResponseDTO>>(packetFishes);
        }

        public async Task<IEnumerable<PacketFishResponseDTO>> GetPacketFishesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
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

            return _mapper.Map<IEnumerable<PacketFishResponseDTO>>(packetFishes);
        }

       
    }
}
