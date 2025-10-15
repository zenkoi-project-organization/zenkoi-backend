using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondService : IPondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Pond> _pondRepo;
        public PondService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pondRepo = _unitOfWork.GetRepo<Pond>();
        }
        public async Task<IEnumerable<PondResponseDTO>> GetAllAsync()
        {
            var pondtypes  = await _pondRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<PondResponseDTO>>(pondtypes);
        }

        public async Task<PondResponseDTO?> GetByIdAsync(int id)
        {
            var pondtypes = await _pondRepo.GetByIdAsync(id);
            return _mapper.Map<PondResponseDTO>(pondtypes);
        }

        public async Task<PondResponseDTO> CreateAsync(PondRequestDTO dto)
        {
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var pondRepo = _unitOfWork.GetRepo<Pond>();
            var pondType = await pondRepo.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.PondTypeId}");
            }

            var entity = _mapper.Map<Pond>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _pondRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PondRequestDTO dto)
        {
            var pond = await _pondRepo.GetByIdAsync(id);
            if (pond == null) return false;
            var areaRepo = _unitOfWork.GetRepo<Area>();
            var area = await areaRepo.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var pondRepo = _unitOfWork.GetRepo<Pond>();
            var pondType = await pondRepo.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.PondTypeId}");
            }

            _mapper.Map(dto, pond);
            await _pondRepo.UpdateAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pond = await _pondRepo.GetByIdAsync(id);
            if (pond == null) return false;

            await _pondRepo.DeleteAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}