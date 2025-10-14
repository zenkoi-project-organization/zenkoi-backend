using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondService : IPondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PondService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PondResponseDTO>> GetAllAsync()
        {
            var pondtypes  = await _unitOfWork.Ponds.GetAllAsync();
            return _mapper.Map<IEnumerable<PondResponseDTO>>(pondtypes);
        }

        public async Task<PondResponseDTO?> GetByIdAsync(int id)
        {
            var pondtypes = await _unitOfWork.Ponds.GetByIdAsync(id);
            return _mapper.Map<PondResponseDTO>(pondtypes);
        }

        public async Task<PondResponseDTO> CreateAsync(PondRequestDTO dto)
        {
            var area = await _unitOfWork.Areas.CheckExistAsync(dto.AreaId);
            if (!area)
            {
                throw new Exception($"không tìm thấy ví trí với AreaId : {dto.AreaId}");
            }
            var pondType = await _unitOfWork.PondTypes.CheckExistAsync(dto.PondTypeId);
            if (!pondType)
            {
                throw new Exception($"không tìm thấy ví trí với PondTypeId : {dto.AreaId}");
            }

            var entity = _mapper.Map<Pond>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Ponds.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PondRequestDTO dto)
        {
            var pond = await _unitOfWork.Ponds.GetByIdAsync(id);
            if (pond == null) return false;

            _mapper.Map(dto, pond);
            await _unitOfWork.Ponds.UpdateAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pond = await _unitOfWork.Ponds.GetByIdAsync(id);
            if (pond == null) return false;

            await _unitOfWork.Ponds.DeleteAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}