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
            var _pondRepo =  _unitOfWork.GetRepo<Pond>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _packetRepo = _unitOfWork.GetRepo<PacketFish>();
            var pond = await _pondRepo.GetSingleAsync(new DAL.Queries.QueryOptions<Pond>
            {
                Predicate = p => p.Id == dto.PondId,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<Pond, object>>>
                {
                    p => p.PondType 
                }
            });
            if(pond == null)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if(pond.PondStatus == PondStatus.Maintenance)
            {
                throw new InvalidOperationException(" hồ hiện tại đang bảo trì");
            }
            var breed = await _breedRepo.GetSingleAsync(new DAL.Queries.QueryOptions<BreedingProcess>
            {
                Predicate = p => p.Id == dto.BreedingProcessId,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<BreedingProcess, object>>>()
                {
                    p => p.ClassificationStage
                }
            }); ;
            if(breed == null)
            {
                throw new InvalidOperationException("không tìm thấy quy trinh sinh sản");
            }
            var packket = await _packetRepo.GetSingleAsync(new DAL.Queries.QueryOptions<PacketFish>
            {
                Predicate = p => p.Id == dto.PacketFishId,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<PacketFish, object>>>
                {
                    p => p.VarietyPacketFishes,
                }
            });
            if(packket == null)
            {
                throw new InvalidOperationException("không tìm thấy loại lô cá");
            }
            var newPackage = _mapper.Map<PondPacketFish>(dto);
            newPackage.Quantity = breed.ClassificationStage.PondQualifiedCount.Value;
            await _repo.CreateAsync(newPackage);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondPacketFishResponseDTO>(newPackage);
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<PondPacketFishResponseDTO>> GetAllPondPacketFishAsync(int pageIndex = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public Task<PondPacketFishResponseDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, PondPacketFishRequestDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
