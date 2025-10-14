using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.DTOs.PondTypeDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PondTypeService : IPondTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PondTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PondTypeResponseDTO>> GetAllAsync()
        {
            var pondtypes  = await _unitOfWork.PondTypes.GetAllAsync();
            return _mapper.Map<IEnumerable<PondTypeResponseDTO>>(pondtypes);
        }

        public async Task<PondTypeResponseDTO?> GetByIdAsync(int id)
        {
            var pondtypes = await _unitOfWork.PondTypes.GetByIdAsync(id);
            return _mapper.Map<PondTypeResponseDTO>(pondtypes);
        }

        public async Task<PondTypeResponseDTO> CreateAsync(PondTypeRequestDTO dto)
        {

            var entity = _mapper.Map<PondType>(dto);
            await _unitOfWork.PondTypes.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<PondTypeResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PondTypeRequestDTO dto)
        {
            var pondtype = await _unitOfWork.PondTypes.GetByIdAsync(id);
            if (pondtype == null) return false;

            _mapper.Map(dto, pondtype);
            _unitOfWork.PondTypes.UpdateAsync(pondtype);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pondtype = await _unitOfWork.PondTypes.GetByIdAsync(id);
            if (pondtype == null) return false;

            _unitOfWork.PondTypes.DeleteAsync(pondtype);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}