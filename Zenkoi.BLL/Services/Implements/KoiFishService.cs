using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Helpers.Fillters;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
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
        private readonly IRepoBase<BreedingProcess> _breedRepo;
        public KoiFishService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
        }
        public async Task<PaginatedList<KoiFishResponseDTO>> GetAllKoiFishAsync(
         KoiFishFilterRequestDTO filter,
         int pageIndex = 1,
         int pageSize = 10)
        {
            var queryOptions = new QueryOptions<KoiFish>
            {
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
            {
                v => v.Variety,
                p => p.Pond
            }
                };

            if (filter != null)
                queryOptions.Predicate = FilterHelper.BuildFilterExpression(filter);

            var koiList = await _koiFishRepo.GetAllAsync(queryOptions);
            var mapped = _mapper.Map<List<KoiFishResponseDTO>>(koiList);

            var totalCount = mapped.Count;
            var paged = mapped
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<KoiFishResponseDTO>(paged, totalCount, pageIndex, pageSize);
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
            if (dto.BreedingProcessId.HasValue)
            {
                var breed = await _breedRepo.GetByIdAsync(dto.BreedingProcessId);
                if (breed == null)
                {
                    throw new Exception("không tìm thấy quy trình sinh sản");
                }

                if (!breed.Status.Equals(BreedingStatus.Complete))
                {
                    throw new Exception("Quy trình sinh sản này chưa hoàn thành");
                }
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