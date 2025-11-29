using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
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
        private readonly IRepoBase<Pond> _pondRepo;
        private readonly IBreedingProcessService _breedingProcessService;
        private readonly IRepoBase<BreedingProcess> _breedRepo;
        private readonly IRepoBase<KoiFavorite> _koiFavoriteRepo;

        public KoiFishService(IUnitOfWork unitOfWork, IMapper mapper, IBreedingProcessService breedingProcessService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
            _pondRepo = _unitOfWork.GetRepo<Pond>();
            _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            _koiFavoriteRepo = _unitOfWork.GetRepo<KoiFavorite>();
            _breedingProcessService = breedingProcessService;
        }


        private void FormatSizeForResponse(KoiFishResponseDTO koi)
        {
            if (double.TryParse(koi.Size?.ToString(), out double cm))
            {
                var inch = CmToInch(cm);
                koi.Size = $"{Math.Round(inch, 1)} inch / {cm} cm";
            }
        }

        public async Task<PaginatedList<KoiFishResponseDTO>> GetAllKoiFishAsync(
            KoiFishFilterRequestDTO filter,
            int pageIndex = 1,
            int pageSize = 10,
            int? userId = null)
        {
            var queryBuilder = new QueryBuilder<KoiFish>()
                .WithTracking(false)
                .WithInclude(v => v.Variety)
                .WithInclude(p => p.Pond)
                .WithOrderBy(q => q.OrderByDescending(k => k.Id));

            if (!string.IsNullOrEmpty(filter.Search))
            {
                string searchLower = filter.Search.ToLower();
                queryBuilder.WithPredicate(k =>
                    (k.Origin != null && k.Origin.ToLower().Contains(searchLower)) ||
                    (k.RFID != null && k.RFID.ToLower().Contains(searchLower)) ||
                    (k.Description != null && k.Description.ToLower().Contains(searchLower)) ||
                    (k.Pond != null && k.Pond.PondName != null && k.Pond.PondName.ToLower().Contains(searchLower)) ||
                    (k.Variety != null && k.Variety.VarietyName != null && k.Variety.VarietyName.ToLower().Contains(searchLower)));
            }

            if (filter.Gender.HasValue)
            {
                queryBuilder.WithPredicate(k => k.Gender == filter.Gender.Value);
            }

            if (filter.Health.HasValue)
            {
                queryBuilder.WithPredicate(k => k.HealthStatus == filter.Health.Value);
            }

            if (filter.IsMutation.HasValue)
            {
                queryBuilder.WithPredicate(k => k.IsMutated == filter.IsMutation.Value);
            }

            if (filter.SaleStatus.HasValue)
            {
                queryBuilder.WithPredicate(k => k.SaleStatus == filter.SaleStatus.Value);
            }
            if (filter.IsBreeding)
            {
                queryBuilder.WithPredicate(k => k.KoiBreedingStatus == KoiBreedingStatus.Ready);
            }

            if (filter.VarietyId.HasValue)
            {
                queryBuilder.WithPredicate(k => k.VarietyId == filter.VarietyId.Value);
            }

            if (filter.PondId.HasValue)
            {
                queryBuilder.WithPredicate(k => k.PondId == filter.PondId.Value);
            }

            if (!string.IsNullOrEmpty(filter.Origin))
            {
                queryBuilder.WithPredicate(k => k.Origin != null && k.Origin.Contains(filter.Origin));
            }

            if (filter.MinPrice.HasValue)
            {
                queryBuilder.WithPredicate(k => k.SellingPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                queryBuilder.WithPredicate(k => k.SellingPrice <= filter.MaxPrice.Value);
            }

            if (filter.MinSize.HasValue)
            {
                queryBuilder.WithPredicate(k => k.Size.HasValue && k.Size.Value >= filter.MinSize.Value);
            }

            if (filter.MaxSize.HasValue)
            {
                queryBuilder.WithPredicate(k => k.Size.HasValue && k.Size.Value <= filter.MaxSize.Value);
            }

            var query = _koiFishRepo.Get(queryBuilder.Build());
            var koiList = await query.ToListAsync();

            bool needSave = false;

            foreach (var koi in koiList)
            {
                var newStatus = CalculateBreedingStatus(koi);

                if (newStatus != koi.KoiBreedingStatus)
                {
                    koi.KoiBreedingStatus = newStatus;
                    _koiFishRepo.UpdateAsync(koi);
                    needSave = true;
                }
            }

            if (needSave)
                await _unitOfWork.SaveAsync();

            var mapped = _mapper.Map<List<KoiFishResponseDTO>>(koiList);

            foreach (var koi in mapped)
                FormatSizeForResponse(koi);

            // Set IsFavorited flag nếu user đã đăng nhập
            HashSet<int> favoriteKoiIds = new HashSet<int>();
            if (userId.HasValue)
            {
                var koiIds = mapped.Select(k => k.Id).ToList();
                var userFavorites = await _koiFavoriteRepo.GetAllAsync(new QueryOptions<KoiFavorite>
                {
                    Predicate = f => f.UserId == userId.Value && koiIds.Contains(f.KoiFishId),
                    Tracked = false
                });

                favoriteKoiIds = userFavorites.Select(f => f.KoiFishId).ToHashSet();

                foreach (var koi in mapped)
                {
                    koi.IsFavorited = favoriteKoiIds.Contains(koi.Id);
                }
            }

            if (filter.IsFavorited.HasValue)
            {
                mapped = mapped.Where(k => k.IsFavorited == filter.IsFavorited.Value).ToList();
            }

            var totalCount = mapped.Count;
            var paged = mapped
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<KoiFishResponseDTO>(paged, totalCount, pageIndex, pageSize);
        }

        public async Task<KoiFishResponseDTO?> GetByIdAsync(int id)
        {
            var koifish = await _koiFishRepo.GetSingleAsync(new QueryOptions<KoiFish>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
                {
                    p => p.BreedingProcess,
                    p => p.Pond,
                    p => p.Variety
                }
            });

            var result = _mapper.Map<KoiFishResponseDTO>(koifish);
            if (result != null)
                FormatSizeForResponse(result);

            return result;
        }
        public async Task<KoiFishResponseDTO?> ScanRFID(string RFID)
        {
            var koifish = await _koiFishRepo.GetSingleAsync(new QueryOptions<KoiFish>
            {
                Predicate = p => p.RFID == RFID,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
                {
                    p => p.BreedingProcess,
                    p => p.Pond,
                    p => p.Variety
                }
            });

            var result = _mapper.Map<KoiFishResponseDTO>(koifish);
            if (result != null)
                FormatSizeForResponse(result);

            return result;
        }

        public async Task<KoiFishResponseDTO> CreateAsync(KoiFishRequestDTO dto)
        {


            var variety = await _varietyRepo.CheckExistAsync(dto.VarietyId);
            if (!variety)
                throw new Exception($"Không tìm thấy variety với id: {dto.VarietyId}");

            var pond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == dto.PondId,
                IncludeProperties = new List<Expression<Func<Pond, object>>> { p => p.KoiFishes,
                p => p.PondType}
            });

            if (pond == null)
                throw new Exception($"Không tìm thấy pond với id {dto.PondId}");

            if (pond.PondType.Type == TypeOfPond.Paring || pond.PondType.Type == TypeOfPond.EggBatch || pond.PondType.Type == TypeOfPond.FryFish)
            {
                throw new InvalidOperationException("vui lòng chọn hồ dành cho cá trưởng thành");
            }

            if (pond.PondStatus == PondStatus.Maintenance)
                throw new InvalidOperationException("Hồ hiện tại đang bảo trì, vui lòng chọn hồ khác.");

            pond.CurrentCount = pond?.KoiFishes.Count();
            if (pond.MaxFishCount < pond.CurrentCount + 1)
                throw new InvalidOperationException("Hồ đã đầy, vui lòng chuyển cá sang hồ khác.");

            if (dto.BreedingProcessId.HasValue)
            {
                var breed = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess>
                {
                    Predicate = p => p.Id == dto.BreedingProcessId,
                    IncludeProperties = new List<Expression<Func<BreedingProcess, object>>>
            {
                p => p.ClassificationStage,
                p => p.MaleKoi,
                p => p.FemaleKoi
            }
                });

                if (breed == null)
                    throw new InvalidOperationException("Không tìm thấy quy trình sinh sản.");

                if (breed.Status != BreedingStatus.Complete && breed.ClassificationStage?.Status != ClassificationStatus.Stage4)
                    throw new InvalidOperationException("Quy trình sinh sản này chưa hoàn thành.");

            }

            var entity = _mapper.Map<KoiFish>(dto);
            pond.PondStatus = PondStatus.Active;
            pond.CurrentCount += 1;

            await _pondRepo.UpdateAsync(pond);
            await _koiFishRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            var res = _mapper.Map<KoiFishResponseDTO>(entity);
            var inch = CmToInch(dto.Size.Value);
            res.Size = $"{Math.Round(inch, 1)} inch / {dto.Size.Value} cm";

            return res;
        }

        public async Task<bool> UpdateAsync(int id, KoiFishUpdateRequestDTO dto)
        {
            var koiFish = await _koiFishRepo.GetByIdAsync(id);
            if (koiFish == null)
                throw new KeyNotFoundException($"Không tìm thấy cá Koi với id {id}.");

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
            else
            {
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

        public async Task<bool> TransferFish(int id, int pondId)
        {
            var koiFish = await _koiFishRepo.GetByIdAsync(id);
            if (koiFish == null)
                throw new KeyNotFoundException("Không tìm thấy cá.");

            if (koiFish.HealthStatus == HealthStatus.Dead)
                throw new InvalidOperationException("Cá đã chết, không thể chuyển hồ.");

            var oldPond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == koiFish.PondId,
                IncludeProperties = new List<Expression<Func<Pond, object>>> { p => p.KoiFishes }
            });

            var newPond = await _pondRepo.GetSingleAsync(new QueryOptions<Pond>
            {
                Predicate = p => p.Id == pondId,
                IncludeProperties = new List<Expression<Func<Pond, object>>> { p => p.KoiFishes }
            });

            if (newPond == null)
                throw new KeyNotFoundException("Không tìm thấy hồ mới.");

            if (newPond.PondStatus == PondStatus.Maintenance)
                throw new InvalidOperationException("Hồ đang bảo trì, không thể chuyển.");

            var newCount = newPond.KoiFishes.Count + 1;
            if (newPond.MaxFishCount.HasValue && newCount > newPond.MaxFishCount.Value)
                throw new InvalidOperationException("Hồ mới đã đầy, không thể chuyển cá vào.");

            koiFish.PondId = pondId;

            if (oldPond != null && oldPond.Id != newPond.Id)
            {
                oldPond.CurrentCount = Math.Max(0, (oldPond.KoiFishes.Count - 1));
            }
            newPond.CurrentCount = newCount;

            await _koiFishRepo.UpdateAsync(koiFish);
            if (oldPond != null) await _pondRepo.UpdateAsync(oldPond);
            await _pondRepo.UpdateAsync(newPond);

            return await _unitOfWork.SaveAsync();
        }

        public async Task<BreedingParentDTO> GetAnalysisAsync(int id)
        {
            var koifish = await _koiFishRepo.GetSingleAsync(new QueryOptions<KoiFish>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
                {
                    p => p.Variety
                }
            });

            var today = DateTime.Now;
            if (koifish == null)
                throw new KeyNotFoundException("không tìm thấy cá koi");

            if (koifish.HealthStatus == HealthStatus.Dead)
                throw new InvalidOperationException("cá đã chết");

            if (koifish.HealthStatus != HealthStatus.Healthy)
                throw new InvalidOperationException("vui lòng chọn cá có thể trạng tốt để có một quá trình sinh sản tốt nhất");

            var historyBreed = await _breedingProcessService.GetKoiFishParentStatsAsync(id);
            var age = (today - koifish.BirthDate.Value).TotalDays / 365.25;

            return new BreedingParentDTO
            {
                Id = koifish.Id,
                RFID = koifish.RFID,
                Variety = koifish.Variety.VarietyName,
                Gender = koifish.Gender.ToString(),
                Size = $"{Math.Round(CmToInch(koifish.Size ?? 0), 1)} inch / {koifish.Size} cm",
                image = koifish.Images[0],
                Health = koifish.HealthStatus.ToString(),
                Age = Math.Round(age, 1),
                IsMutated = koifish.IsMutated,
                MutationDescription = koifish.MutationDescription,
                BreedingHistory = new List<BreedingRecordDTO>
                {
                    new BreedingRecordDTO
                    {
                        FertilizationRate = historyBreed.FertilizationRate,
                        HatchRate = historyBreed.HatchRate,
                        SurvivalRate = historyBreed.SurvivalRate,
                        HighQualifiedRate = historyBreed.HighQualifiedRate,
                        ResultNote = $"Participations: {historyBreed.ParticipationCount}, Failed: {historyBreed.FailCount}",
                    }
                }
            };
        }


        public static double CmToInch(double cm)
        {
            return cm / 2.54;
        }
        public int GetAgeInMonths(DateTime birthDate)
        {
            var today = DateTime.UtcNow;
            return ((today.Year - birthDate.Year) * 12) + today.Month - birthDate.Month;
        }
        public KoiBreedingStatus CalculateBreedingStatus(KoiFish fish)
        {
            var ageMonths = GetAgeInMonths(fish.BirthDate.Value);

            // Nếu dưới 24 tháng → luôn là NotMature
            if (ageMonths < 24)
                return KoiBreedingStatus.NotMature;

            // Nếu đủ 24 tháng trở lên → để staff tự quản lý trạng thái
            return fish.KoiBreedingStatus;
        }

    }
}
