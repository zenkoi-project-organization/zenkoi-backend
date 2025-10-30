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
                p => p.Pond,
            }
                };
      
           Expression<Func<KoiFish, bool>>? predicate = null;

            if (!string.IsNullOrEmpty(filter.Search))
            {
                string searchLower = filter.Search.ToLower();

                Expression<Func<KoiFish, bool>> expr = k =>                 
                    (k.Origin != null && k.Origin.ToLower().Contains(searchLower)) ||
                    (k.BodyShape != null && k.BodyShape.ToLower().Contains(searchLower)) ||
                    (k.RFID != null && k.RFID.ToLower().Contains(searchLower)) ||
                    (k.Description != null && k.Description.ToLower().Contains(searchLower)) ||
             
                    (k.Pond != null && k.Pond.PondName != null && k.Pond.PondName.ToLower().Contains(searchLower)) || 
                    (k.Variety != null && k.Variety.VarietyName != null && k.Variety.VarietyName.ToLower().Contains(searchLower));

                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            if (filter.Gender.HasValue)
            {
                Expression<Func<KoiFish, bool>> expr = k => k.Gender == filter.Gender.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
     
            if (filter.Health.HasValue)
            {      
                Expression<Func<KoiFish, bool>> expr = k => k.HealthStatus == filter.Health.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.SaleStatus.HasValue)
            {
                Expression<Func<KoiFish, bool>> expr = k => k.SaleStatus == filter.SaleStatus.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
       
            if (filter.VarietyId.HasValue)
            {
                Expression<Func<KoiFish, bool>> expr = k => k.VarietyId == filter.VarietyId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.FishSize.HasValue)
            {
                Expression<Func<KoiFish, bool>> expr = k => k.Size == filter.FishSize.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
        
            if (filter.PondId.HasValue)
            {
                Expression<Func<KoiFish, bool>> expr = k => k.PondId == filter.PondId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
      
            if (!string.IsNullOrEmpty(filter.Origin))
            {
                Expression<Func<KoiFish, bool>> expr = k => k.Origin == filter.Origin;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }          
            if (filter.MinPrice.HasValue)
            {         
                Expression<Func<KoiFish, bool>> expr = k => k.SellingPrice >= filter.MinPrice.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
       
            if (filter.MaxPrice.HasValue)
            {             
                Expression<Func<KoiFish, bool>> expr = k => k.SellingPrice <= filter.MaxPrice.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

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
            var koifish = await _koiFishRepo.GetSingleAsync(new QueryOptions<KoiFish> { 
                Predicate = p =>p.Id == id, 
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
                {
                    p => p.BreedingProcess,
                    p => p.Pond,
                    p => p.Variety
                }                
                });

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

        public async Task<bool> UpdateAsync(int id, KoiFishUpdateRequestDTO dto)
        {
            var koiFish = await _koiFishRepo.GetByIdAsync(id);
            if (koiFish == null)
                throw new Exception($"Không tìm thấy cá Koi với id {id}.");

            if (dto.VarietyId.HasValue)
            {
                var varietyExists = await _varietyRepo.CheckExistAsync(dto.VarietyId.Value);
                if (!varietyExists)
                    throw new Exception($"Không tìm thấy Variety với id: {dto.VarietyId}");
            }

            if (dto.PondId.HasValue)
            {
                var pondExists = await _pondRepo.CheckExistAsync(dto.PondId.Value);
                if (!pondExists)
                    throw new Exception($"Không tìm thấy Pond với id: {dto.PondId}");
            }

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

        public async Task<KoiFishFamilyResponseDTO> GetFamilyTreeAsync(int koiId)
        {
            var options = new QueryOptions<KoiFish>
            {
                Predicate = k => k.Id == koiId,
                Tracked = false,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
            {
                k => k.BreedingProcess,
                k => k.Variety,
                k => k.BreedingProcess.MaleKoi,
                k => k.BreedingProcess.FemaleKoi,
                k => k.BreedingProcess.MaleKoi.Variety,
                k => k.BreedingProcess.FemaleKoi.Variety,
                k => k.BreedingProcess.MaleKoi.BreedingProcess,
                k => k.BreedingProcess.FemaleKoi.BreedingProcess,
                k => k.BreedingProcess.MaleKoi.BreedingProcess.MaleKoi,
                k => k.BreedingProcess.MaleKoi.BreedingProcess.FemaleKoi,
                k => k.BreedingProcess.MaleKoi.BreedingProcess.MaleKoi.Variety,
                k => k.BreedingProcess.MaleKoi.BreedingProcess.FemaleKoi.Variety,
                k => k.BreedingProcess.FemaleKoi.BreedingProcess.MaleKoi,
                k => k.BreedingProcess.FemaleKoi.BreedingProcess.FemaleKoi,
                k => k.BreedingProcess.FemaleKoi.BreedingProcess.MaleKoi.Variety,
                k => k.BreedingProcess.FemaleKoi.BreedingProcess.FemaleKoi.Variety,
            }
            };

            var koi = await _koiFishRepo.GetSingleAsync(options);

            if (koi == null)
                throw new Exception("Không tìm thấy cá Koi.");

            if (koi.BreedingProcess == null)
            {
                return new KoiFishFamilyResponseDTO
                {
                    Id = koi.Id,
                    RFID = koi.RFID,
                    VarietyName = koi.Variety?.VarietyName,
                    Gender = koi.Gender,
                };

            }
            else {
                var breeding = koi.BreedingProcess;


                var response = new KoiFishFamilyResponseDTO
                {
                    Id = koi.Id,
                    RFID = koi.RFID,
                    VarietyName = koi.Variety?.VarietyName,
                    Gender = koi.Gender,
                    Father = breeding.MaleKoi != null ? new KoiParentDTO
                    {
                        Id = breeding.MaleKoi.Id,
                        RFID = breeding.MaleKoi.RFID,
                        VarietyName = breeding.MaleKoi.Variety?.VarietyName,
                        Gender = breeding.MaleKoi.Gender,
                        Father = breeding.MaleKoi.BreedingProcess?.MaleKoi != null
                            ? _mapper.Map<KoiGrandParentDTO>(breeding.MaleKoi.BreedingProcess.MaleKoi)
                            : null,
                        Mother = breeding.MaleKoi.BreedingProcess?.FemaleKoi != null
                            ? _mapper.Map<KoiGrandParentDTO>(breeding.MaleKoi.BreedingProcess.FemaleKoi)
                            : null
                    } : null,
                    Mother = breeding.FemaleKoi != null ? new KoiParentDTO
                    {
                        Id = breeding.FemaleKoi.Id,
                        RFID = breeding.FemaleKoi.RFID,
                        VarietyName = breeding.FemaleKoi.Variety?.VarietyName,
                        Gender = breeding.FemaleKoi.Gender,
                        Father = breeding.FemaleKoi.BreedingProcess?.MaleKoi != null
                            ? _mapper.Map<KoiGrandParentDTO>(breeding.FemaleKoi.BreedingProcess.MaleKoi)
                            : null,
                        Mother = breeding.FemaleKoi.BreedingProcess?.FemaleKoi != null
                            ? _mapper.Map<KoiGrandParentDTO>(breeding.FemaleKoi.BreedingProcess.FemaleKoi)
                            : null
                    } : null
                };

                return response;
            }
        }
    }
}