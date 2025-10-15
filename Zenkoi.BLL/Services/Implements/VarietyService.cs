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
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class VarietyService : IVarietyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Variety> _varietyRepo;
        public VarietyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
        }
        public async Task<IEnumerable<VarietyResponseDTO>> GetAllAsync()
        {
            var varieties = await _varietyRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<VarietyResponseDTO>>(varieties);
        }

        public async Task<VarietyResponseDTO?> GetByIdAsync(int id)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            return _mapper.Map<VarietyResponseDTO>(variety);
        }

        public async Task<VarietyResponseDTO> CreateAsync(VarietyRequestDTO dto)
        {
          

            var entity = _mapper.Map<Variety>(dto);
           
            await _varietyRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<VarietyResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, VarietyRequestDTO dto)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            if (variety == null) return false;

            _mapper.Map(dto, variety);
            await _varietyRepo.UpdateAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var variety = await _varietyRepo.GetByIdAsync(id);
            if (variety == null) return false;

            await _varietyRepo.DeleteAsync(variety);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}