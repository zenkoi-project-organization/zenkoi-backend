using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PondPacketFishDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;

using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.FilterDTOs;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondPacketFishService : IPondPacketFishService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<PondPacketFish> _repo; 
        public PondPacketFishService(IUnitOfWork  unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repo = _unitOfWork.GetRepo<PondPacketFish>();
        }
        public async Task<PondPacketFishResponseDTO> CreateAsync(PondPacketFishRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _packetRepo = _unitOfWork.GetRepo<PacketFish>();

            var breed = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess>
            {
                Predicate = p => p.Id == dto.BreedingProcessId,
                IncludeProperties = new List<Expression<Func<BreedingProcess, object>>>
        {
            p => p.ClassificationStage
        }
            });

            if (breed == null)
                throw new InvalidOperationException("không tìm thấy quy trình sinh sản");

            if (breed.ClassificationStage == null)
                throw new InvalidOperationException("Lô cá chưa phân loại");

            var packet = await _packetRepo.GetSingleAsync(new QueryOptions<PacketFish>
            {
                Predicate = p => p.Id == dto.PacketFishId,
                IncludeProperties = new List<Expression<Func<PacketFish, object>>>
        {
            p => p.VarietyPacketFishes
        }
            });

            if (packet == null)
                throw new InvalidOperationException("Không tìm thấy loại lô cá");

            var newPackage = _mapper.Map<PondPacketFish>(dto);
            newPackage.QuantityFish = breed.ClassificationStage.PondQualifiedCount.Value;
            newPackage.QuantityPacket = newPackage.QuantityFish / packet.FishPerPacket;
            packet.StockQuantity = newPackage.QuantityPacket;
            if (dto.PondId != breed.PondId)
            {
                var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
                {
                    Predicate = p => p.Id == dto.PondId,
                    IncludeProperties = new List<Expression<Func<Pond, object>>>
            {
                p => p.PondType
            }
                });

                if (pond == null)
                    throw new KeyNotFoundException("Không tìm thấy hồ");

                if (pond.PondStatus == PondStatus.Maintenance)
                    throw new InvalidOperationException("Hồ hiện tại đang bảo trì");

                if (breed.ClassificationStage.PondQualifiedCount > pond.MaxFishCount)
                    throw new InvalidOperationException("Số lượng cá vượt sức chứa của hồ");

                pond.PondStatus = PondStatus.Active;
                await _pondRepo.UpdateAsync(pond);
            }

            await _repo.CreateAsync(newPackage);
            await _packetRepo.UpdateAsync(packet);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PondPacketFishResponseDTO>(newPackage);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();

            var packet = await _repo.GetByIdAsync(id);
            if (packet == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lô cá để xóa");
            }
            var pond = await _pondRepo.GetByIdAsync(packet.PondId);
            if (pond == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hồ chứa lô cá này");
            }

            await _repo.DeleteAsync(packet);
        
            pond.PondStatus = PondStatus.Empty;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<PaginatedList<PondPacketFishResponseDTO>> GetAllPondPacketFishAsync(PondPacketFishFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {

            Expression<Func<PondPacketFish, bool>>? predicate = null;

            if (filter.BreedingProcessId.HasValue)
            {
                Expression<Func<PondPacketFish, bool>> expr = k => k.BreedingProcessId == filter.BreedingProcessId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            var Packets = await _repo.GetAllAsync(new QueryOptions<PondPacketFish>
            {
                Predicate = predicate, 
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<PondPacketFish, object>>> {
                    x => x.BreedingProcess,
                    x => x.Pond,
                    x => x.PacketFish
                }
            });

            var mappedList = _mapper.Map<List<PondPacketFishResponseDTO>>(Packets);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PondPacketFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PondPacketFishResponseDTO> GetByIdAsync(int id)
        {
            var pondPacket = await _repo.GetSingleAsync(new QueryOptions<PondPacketFish>
            {
                Predicate = x => x.Id == id , IncludeProperties = new List<System.Linq.Expressions.Expression<Func<PondPacketFish, object>>> { 
                    x => x.BreedingProcess,
                    x => x.Pond,
                    x => x.PacketFish
                },
            });
           if (pondPacket == null)
            {
                throw new KeyNotFoundException("không tìm thấy lô cá");
            }
           return _mapper.Map<PondPacketFishResponseDTO>(pondPacket);
        }

        public async Task<bool> TranferPacket(int id, PondPacketFishUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();

            var packet = await _repo.GetByIdAsync(id);
            if (packet == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lô cá");
            }

            if (packet.PondId == dto.PondId)
            {
                throw new InvalidOperationException("Hồ đích trùng với hồ hiện tại, không cần chuyển");
            }

            var newPond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = x => x.Id == dto.PondId,
                IncludeProperties = new List<Expression<Func<Pond, object>>>
        {
            p => p.PondType
        }
            });

            if (newPond == null)
                throw new KeyNotFoundException("Không tìm thấy hồ đích");

            if (newPond.MaxFishCount < packet.QuantityFish)
                throw new InvalidOperationException("Số lượng cá vượt sức chứa của hồ đích");

            if (newPond.PondStatus == PondStatus.Maintenance || newPond.PondStatus == PondStatus.Active)
                throw new InvalidOperationException($"Hồ đích hiện tại đang {newPond.PondStatus}, không thể chuyển cá vào");

            var oldPond = await _pondRepo.GetByIdAsync(packet.PondId);

            packet.PondId = dto.PondId;

            if (oldPond != null)
            {
                oldPond.PondStatus = PondStatus.Empty;
            }

            newPond.PondStatus = PondStatus.Active;

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

    }
}
