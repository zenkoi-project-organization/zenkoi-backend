using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class KoiFishService : IKoiFishService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<Variety> _varietyRepo;
        private readonly IRepoBase<Pond>  _pondRepo;
        public KoiFishService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
        }
        public async Task<IEnumerable<KoiFishResponseDTO>> GetAllAsync()
        {
            var koifishes = await _koiFishRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<KoiFishResponseDTO>>(koifishes);
        }

        public async Task<KoiFishResponseDTO?> GetByIdAsync(int id)
        {
            var koifish = await _koiFishRepo.GetByIdAsync(id);
            return _mapper.Map<KoiFishResponseDTO>(koifish);
        }

        public async Task<KoiFishResponseDTO> CreateAsync(KoiFishRequestDTO dto)
        {
            var variety = await _varietyRepo.CheckExistAsync(dto.VarietyId);
            if (!variety)
            {
                throw new Exception($"không tìm thấy variety với id : {dto.VarietyId}");
            }
            var pond = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pond){
                throw new Exception($"không tìm thấy pond với id {dto.PondId}");
            }

            var entity = _mapper.Map<KoiFish>(dto);

            await _koiFishRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<KoiFishResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, KoiFishRequestDTO dto)
        {
            var variety = await _varietyRepo.CheckExistAsync(dto.VarietyId);
            if (!variety)
            {
                throw new Exception($"không tìm thấy variety với id : {dto.VarietyId}");
            }
            var pond = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pond)
            {
                throw new Exception($"không tìm thấy pond với id {dto.PondId}");
            }
            var koiFish = await _koiFishRepo.GetByIdAsync(id);
            if (koiFish == null) return false;

            _mapper.Map(dto, koiFish);
            await _koiFishRepo.UpdateAsync(koiFish);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var koifish = await _koiFishRepo.GetByIdAsync(id);
            if (koifish == null) return false;

            await _koiFishRepo.DeleteAsync(koifish);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}