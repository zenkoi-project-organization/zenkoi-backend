using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class VarietyService : IVarietyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public VarietyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<VarietyResponseDTO>> GetAllAsync()
        {
            var varieties = await _unitOfWork.Varieties.GetAllAsync();
            return _mapper.Map<IEnumerable<VarietyResponseDTO>>(varieties);
        }

        public async Task<VarietyResponseDTO?> GetByIdAsync(int id)
        {
            var variety = await _unitOfWork.Varieties.GetByIdAsync(id);
            return _mapper.Map<VarietyResponseDTO>(variety);
        }

        public async Task<VarietyResponseDTO> CreateAsync(VarietyRequestDTO dto)
        {
          

            var entity = _mapper.Map<Variety>(dto);
           
            await _unitOfWork.Varieties.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<VarietyResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, VarietyRequestDTO dto)
        {
            var variety = await _unitOfWork.Varieties.GetByIdAsync(id);
            if (variety == null) return false;

            _mapper.Map(dto, variety);
            await _unitOfWork.Varieties.UpdateAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variety = await _unitOfWork.Varieties.GetByIdAsync(id);
            if (variety == null) return false;

            await _unitOfWork.Varieties.DeleteAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}