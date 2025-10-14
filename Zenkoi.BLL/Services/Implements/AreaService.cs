using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<AreaResponseDTO>> GetAllAsync()
        {
            var areas = await _unitOfWork.Areas.GetAllAsync();
            return _mapper.Map<IEnumerable<AreaResponseDTO>>(areas);
        }

        public async Task<AreaResponseDTO?> GetByIdAsync(int id)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(id);
            return _mapper.Map<AreaResponseDTO>(area);
        }

        public async Task<AreaResponseDTO> CreateAsync(AreaRequestDTO dto)
        {
            
            var entity = _mapper.Map<Area>(dto);
            await _unitOfWork.Areas.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AreaResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, AreaRequestDTO dto)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(id);
            if (area == null) return false;

            _mapper.Map(dto, area);
            _unitOfWork.Areas.UpdateAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(id);
            if (area == null) return false;

            _unitOfWork.Areas.DeleteAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}