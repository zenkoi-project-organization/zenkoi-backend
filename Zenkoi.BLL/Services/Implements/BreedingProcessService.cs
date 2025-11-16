using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.BreedingDTOs;
using Zenkoi.BLL.DTOs.PondDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Enums;
using System.Data;
using System.Linq.Expressions;
using Zenkoi.BLL.DTOs.KoiFishDTOs;
using Zenkoi.BLL.DTOs.AIBreedingDTOs;
using Newtonsoft.Json;

namespace Zenkoi.BLL.Services.Implements
{
    public class BreedingProcessService : IBreedingProcessService
    {
        private readonly ConcurrentDictionary<(int a, int b), double> _memoKinship = new();
        private readonly ConcurrentDictionary<int, double> _memoF = new();
        private readonly int _maxDepth;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<BreedingProcess> _breedRepo;
        public BreedingProcessService(IUnitOfWork unitOfWork, IMapper mapper, int maxDepth = 30)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _maxDepth = Math.Max(1, maxDepth);
            _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
        }

        public async Task<double> GetOffspringInbreedingAsync(int maleId, int femaleId)
        {
            // F_offspring = kinship(male, female)
            var _koirepo = _unitOfWork.GetRepo<KoiFish>();
            // check exsit 
            var male = await _koirepo.CheckExistAsync(maleId);
            if (!male)
            {
                throw new KeyNotFoundException($"không tìm thấy cá trống với id : {maleId}");
            }
            var female = await _koirepo.CheckExistAsync(femaleId);
            if (!female)
            {
                throw new KeyNotFoundException($"không tìm thấy cá mái với id :{maleId}");
            }


            return await GetKinshipAsync(maleId, femaleId, depth: 0);
        }

        public async Task<double> GetIndividualInbreedingAsync(int koiId)
        {
            if (_memoF.TryGetValue(koiId, out var cached)) return cached;

            var _koirepo = _unitOfWork.GetRepo<KoiFish>();
            // check exsit 
            var male = await _koirepo.CheckExistAsync(koiId);
            if (!male)
            {
                throw new KeyNotFoundException($"không tìm thấy với id : {koiId}");
            }

            var parents = await GetParentsAsync(koiId);
            double result = 0.0;

            if (parents.sireId != null && parents.damId != null)
            {
                result = await GetKinshipAsync(parents.sireId.Value, parents.damId.Value, 0 );
            }

            _memoF[koiId] = result;
            return result;
        }



        private async Task<double> GetKinshipAsync(int xId, int yId, int depth)
        {
            // Chuẩn hóa khóa (a <= b) vì φ đối xứng.
            var key = xId <= yId ? (xId, yId) : (yId, xId);
            if (_memoKinship.TryGetValue(key, out var cached)) return cached;

            if (depth > _maxDepth)
            {
                // Cắt độ sâu để tránh đệ quy quá sâu khi pedigree lớn/khuyết.
                _memoKinship[key] = 0.0;
                return 0.0;
            }

            if (xId == yId)
            {
                var Fx = await GetIndividualInbreedingAsync(xId);
                var selfKin = 0.5 * (1.0 + Fx);
                _memoKinship[key] = selfKin;
                return selfKin;
            }

            var px = await GetParentsAsync(xId);
            var py = await GetParentsAsync(yId);

            bool xFounder = px.sireId is null || px.damId is null;
            bool yFounder = py.sireId is null || py.damId is null;

            double value;

            if (xFounder && yFounder)
            {
                // Giả định founder không họ hàng với nhau.
                value = 0.0;
            }
            else if (!xFounder)
            {
                // Mở rộng phía X (ưu tiên mở rộng phía có bố/mẹ)
                var left = await GetKinshipAsync(px.sireId!.Value, yId, depth + 1 );
                var right = await GetKinshipAsync(px.damId!.Value, yId, depth + 1 );
                value = 0.5 * (left + right);
            }
            else
            {
                // X là founder, mở rộng phía Y
                var left = await GetKinshipAsync(xId, py.sireId!.Value, depth + 1 );
                var right = await GetKinshipAsync(xId, py.damId!.Value, depth + 1 );
                value = 0.5 * (left + right);
            }

            _memoKinship[key] = value;
            return value;
        }
        private async Task<(int? sireId, int? damId)> GetParentsAsync(int koiId)
        {
            
            var _koifish = _unitOfWork.GetRepo<KoiFish>();
            var koifish = await _koifish.GetSingleAsync(new QueryOptions<KoiFish>
            {
                Predicate = KoiFish => KoiFish.Id == koiId,
            });

            if (koifish == null || koifish.BreedingProcessId == null)
                return (null, null); // founder hoặc chưa biết


            var bp = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess> 
            { Predicate = BreedingProcess => BreedingProcess.Id == koifish.BreedingProcessId
            });


            if (bp == null) return (null, null);

            return (bp.MaleKoiId, bp.FemaleKoiId);
        }

        public async Task<BreedingProcessResponseDTO> AddBreeding(BreedingProcessRequestDTO dto)
        {
            var _koifish = _unitOfWork.GetRepo<KoiFish>();
            var _pondRepo  = _unitOfWork.GetRepo<Pond>();
            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if(pond == null)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty)){
                throw new Exception("hiện tại hồ bạn chọn không trống");
            }
            var malekoi = await _koifish.GetByIdAsync(dto.MaleKoiId);
            if (malekoi == null)
            {
                throw new KeyNotFoundException("không tìm thấy cá trống");
            }
            var femalekoi = await _koifish.GetByIdAsync(dto.FemaleKoiId);
            if(femalekoi == null) {
                throw new KeyNotFoundException("không tìm thấy cá mái");
            }
            if (malekoi.Gender.Equals(femalekoi.Gender)) 
            {
                throw new Exception("vui lòng chọn đúng một cặp cá koi (cá trống và cá mái)");
            }
            pond.PondStatus = PondStatus.Active;
            var entity = _mapper.Map<BreedingProcess>(dto);
            entity.StartDate = DateTime.UtcNow;
            entity.Status = BreedingStatus.Pairing;
            entity.Result = BreedingResult.Unknown;
                     
            entity.Code = await GenerateBreedingProcessCodeAsync();
            
            await _breedRepo.CreateAsync(entity);
            await _pondRepo.UpdateAsync(pond);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BreedingProcessResponseDTO>(entity);

        }

        public async Task<BreedingProcessResponseDTO> GetBreedingById(int id)
        {
            var breeding = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess>
            {
                Predicate = b => b.Id == id,
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<BreedingProcess, object>>>
        {
                b => b.MaleKoi,
                b => b.FemaleKoi,
                p => p.MaleKoi!.Variety,
                p => p.FemaleKoi!.Variety,
                b => b.Pond,
                b => b.Batch
        }
            });

            if (breeding == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản");
            }

            var res = _mapper.Map<BreedingProcessResponseDTO>(breeding);
            res.HatchedTime = breeding.Batch.HatchingTime;
            return res;
        }

        public async Task<PaginatedList<BreedingProcessResponseDTO>> GetAllBreedingProcess(
      BreedingProcessFilterRequestDTO filter, int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<BreedingProcess>
            {
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<BreedingProcess, object>>>
            {
                b => b.MaleKoi,
                b => b.FemaleKoi,
                b => b.Pond,
                b => b.MaleKoi.Variety,
                b => b.FemaleKoi.Variety,
                b => b.Batch
            }
            };

            System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>>? predicate = null;

            // SEARCH chung (Id / Code / VarietyName / OriginCountry)
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var search = filter.Search.Trim();

                if (int.TryParse(search, out var koiId))
                {
                    System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr =
                        b => b.MaleKoiId == koiId || b.FemaleKoiId == koiId || b.Id == koiId;

                    predicate = predicate == null ? expr : predicate.AndAlso(expr);
                }
                else
                {
                    System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr =
                        b =>
                            (b.Code != null && b.Code.Contains(search)) ||
                            (b.MaleKoi != null && b.MaleKoi.Variety != null &&
                             b.MaleKoi.Variety.VarietyName != null && b.MaleKoi.Variety.VarietyName.Contains(search)) ||
                            (b.FemaleKoi != null && b.FemaleKoi.Variety != null &&
                             b.FemaleKoi.Variety.VarietyName != null && b.FemaleKoi.Variety.VarietyName.Contains(search)) ||
                            (b.MaleKoi != null && b.MaleKoi.Variety != null &&
                             b.MaleKoi.Variety.OriginCountry != null && b.MaleKoi.Variety.OriginCountry.Contains(search)) ||
                            (b.FemaleKoi != null && b.FemaleKoi.Variety != null &&
                             b.FemaleKoi.Variety.OriginCountry != null && b.FemaleKoi.Variety.OriginCountry.Contains(search));

                    predicate = predicate == null ? expr : predicate.AndAlso(expr);
                }
            }

            // Lọc theo Code (explicit)
            if (!string.IsNullOrEmpty(filter.Code))
            {
                var code = filter.Code.Trim();
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.Code != null && b.Code.Contains(code);
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            // Các filter id cơ bản
            if (filter.MaleKoiId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.MaleKoiId == filter.MaleKoiId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.FemaleKoiId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.FemaleKoiId == filter.FemaleKoiId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.PondId.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.PondId == filter.PondId.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // Status, Result
            if (filter.Status.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.Status == filter.Status.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.Result.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.Result == filter.Result.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // Tổng cá đạt chuẩn / gói
            if (filter.MinTotalFishQualified.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalFishQualified >= filter.MinTotalFishQualified.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxTotalFishQualified.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalFishQualified <= filter.MaxTotalFishQualified.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MinTotalPackage.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalPackage >= filter.MinTotalPackage.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxTotalPackage.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalPackage <= filter.MaxTotalPackage.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // TotalEggs
            if (filter.MinTotalEggs.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalEggs >= filter.MinTotalEggs.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxTotalEggs.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.TotalEggs <= filter.MaxTotalEggs.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // FertilizationRate (double?)
            if (filter.MinFertilizationRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.FertilizationRate >= filter.MinFertilizationRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxFertilizationRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.FertilizationRate <= filter.MaxFertilizationRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // CurrentSurvivalRate (double?)
            if (filter.MinCurrentSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.SurvivalRate >= filter.MinCurrentSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.MaxCurrentSurvivalRate.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.SurvivalRate <= filter.MaxCurrentSurvivalRate.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            // Date ranges
            if (filter.StartDateFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.StartDate >= filter.StartDateFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.StartDateTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.StartDate <= filter.StartDateTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.EndDateFrom.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.EndDate >= filter.EndDateFrom.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }
            if (filter.EndDateTo.HasValue)
            {
                System.Linq.Expressions.Expression<System.Func<BreedingProcess, bool>> expr = b => b.EndDate <= filter.EndDateTo.Value;
                predicate = predicate == null ? expr : predicate.AndAlso(expr);
            }

            queryOptions.Predicate = predicate;

            var allBreeds = await _breedRepo.GetAllAsync(queryOptions);
            Console.WriteLine($"Số lượng BreedingProcess: {allBreeds.Count()}");

            var mappedList = allBreeds.Select(bp => _mapper.Map<BreedingProcessResponseDTO>(bp)).ToList();

            var count = mappedList.Count;
            var items = mappedList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<BreedingProcessResponseDTO>(items, count, pageIndex, pageSize);
        }


        private async Task<string> GenerateBreedingProcessCodeAsync()
        {      
            var allBreeds = await _breedRepo.GetAll();
            var count = allBreeds.Count();
            var nextNumber = count + 1;
            return $"BP-{nextNumber}";
        }
           
        public async Task<bool> UpdateStatus(int id)
        {
            var breed = await _breedRepo.GetByIdAsync(id);
            if (breed == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản ");
            }
            if (!breed.Status.Equals(BreedingStatus.Pairing))
            {
                throw new Exception($"vui lòng cập nhật với breeding với status {BreedingStatus.Pairing}");
            }
            breed.Status = BreedingStatus.Spawned;
            await _breedRepo.UpdateAsync(breed);
            return await _unitOfWork.SaveAsync();
        }

        public async Task<BreedingResponseDTO> GetDetailBreedingById(int id)
        {
            var options = new QueryOptions<BreedingProcess>
            {
                Predicate = p => p.Id == id,
                IncludeProperties = new List<Expression<Func<BreedingProcess, object>>>
            {
                    p => p.MaleKoi!.Variety,
                    p => p.FemaleKoi!.Variety,
                    p => p.Pond,
                    p => p.Batch,                          
                    p => p.Batch!.IncubationDailyRecords,  
                    p => p.FryFish,                        
                    p => p.FryFish!.FrySurvivalRecords,    
                    p => p.ClassificationStage,           
                    p => p.ClassificationStage!.ClassificationRecords,
                }
              };
            var breed = await _breedRepo.GetSingleAsync(options);
            var res =  _mapper.Map<BreedingResponseDTO>(breed);
            res.HatchedTime = breed.Batch?.HatchingTime;
            return res;
        }

        public async Task<KoiFishParentResponseDTO> GetKoiFishParentStatsAsync(int koiFishId)
        {
            var options = new QueryOptions<BreedingProcess>
            {
                Predicate = bp =>
                    (bp.MaleKoiId == koiFishId || bp.FemaleKoiId == koiFishId) &&
                    (bp.Status == BreedingStatus.Complete || bp.Status == BreedingStatus.Failed),
                Tracked = false
            };

            var breedings = await _breedRepo.GetAllAsync(options);

            if (!breedings.Any())
            {
                return new KoiFishParentResponseDTO
                {
                    KoiFishId = koiFishId,
                    ParticipationCount = 0,
                    FailCount = 0
                };
            }

            // 🧮 Tính toán cơ bản
            var response = new KoiFishParentResponseDTO
            {
                KoiFishId = koiFishId,
                ParticipationCount = breedings.Count(),
                FailCount = breedings.Count(b => b.Status == BreedingStatus.Failed),

                FertilizationRate = breedings.Average(b => b.FertilizationRate ?? 0),
                HatchRate = breedings.Average(b => b.HatchingRate ?? 0),
                SurvivalRate = breedings.Average(b => b.SurvivalRate ?? 0),
                HighQualifiedRate = breedings
                    .Where(b => (b.SurvivalRate ?? 0) > 0 && (b.HatchingRate ?? 0) > 0 && (b.TotalEggs ?? 0) > 0)
                    .Average(b =>
                        b.TotalFishQualified /
                        ((b.SurvivalRate ?? 0) * (b.HatchingRate ?? 0) * (b.TotalEggs ?? 1.0))
                    )
            };

            // 🧬 Tính trung bình tỷ lệ đột biến (nếu có)
            if (breedings.Any(b => b.MutationRate.HasValue))
            {
                response.AverageMutationRate = breedings.Average(b => b.MutationRate ?? 0);
            }

            return response;
        }


        public async Task<List<BreedingParentDTO>> GetParentsWithPerformanceAsync(string? variety = null)
        {
            var today = DateTime.Now;

            // ✅ Tạo QueryOptions để lọc koi trong độ tuổi sinh sản
            var options = new QueryOptions<KoiFish>
            {
                Predicate = k =>
                k.Gender != Gender.Other &&
                k.HealthStatus != HealthStatus.Weak &&
                k.BirthDate.HasValue &&
                EF.Functions.DateDiffYear(k.BirthDate.Value, today) > 2 &&
                EF.Functions.DateDiffYear(k.BirthDate.Value, today) <= 6 &&
                (string.IsNullOrEmpty(variety) || k.Variety.VarietyName == variety),

                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
    {
        k => k.Variety
    },
                Tracked = false
            };

            var koiRepo = _unitOfWork.GetRepo<KoiFish>();
            var koiList = await koiRepo.GetAllAsync(options);

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(koiList,
              new System.Text.Json.JsonSerializerOptions
              {
                  WriteIndented = true,
                  ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
              }));

            var result = new List<BreedingParentDTO>();
            foreach (var k in koiList)
            {
                var perf = await GetKoiFishParentStatsAsync(k.Id);

                var age = (today - k.BirthDate.Value).TotalDays / 365.25;

                result.Add(new BreedingParentDTO
                {
                    Id = k.Id,
                    RFID = k.RFID,
                    Variety = k.Variety.VarietyName,
                    Gender = k.Gender.ToString(),
                    IsMutated = k.IsMutated,
                    MutationRate = k.MutationRate,
                    MutationDescription = k.MutationDescription,
                    Size = k.Size.ToString(),
                    image = k.Images[0],
                    Health = k.HealthStatus.ToString(),
                    Age = Math.Round(age, 1),
                    BreedingHistory = new List<BreedingRecordDTO>
            {
                new BreedingRecordDTO
                {
                    FertilizationRate = perf.FertilizationRate,
                    HatchRate = perf.HatchRate,
                    SurvivalRate = perf.SurvivalRate,
                    HighQualifiedRate = perf.HighQualifiedRate,
                    ResultNote = $"Participations: {perf.ParticipationCount}, Failed: {perf.FailCount}"
                }
            }
                });
            }

            return result;
        }

        public async Task<bool> CancelBreeding(int id)
        { 
            var breed = await _breedRepo.GetByIdAsync(id);
            if (breed == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản ");
            }
            if (breed.Status.Equals(BreedingStatus.Complete))
            {
                throw new Exception($"hiện tại quá trình sinh sản này đã hoàn thành nên không thể hủy được");
            }
            breed.Status = BreedingStatus.Failed;
            await _breedRepo.UpdateAsync(breed);
            return await _unitOfWork.SaveAsync();
        }

        public async Task<List<KoiFishResponseDTO>> GetAllKoiFishByBreedingProcessAsync(int breedingProcessId)
        {
            var _koiRepo = _unitOfWork.GetRepo<KoiFish>();
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();

            var breeding = await _breedRepo.GetByIdAsync(breedingProcessId);
            if (breeding == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản.");
            }

            var koiList = await _koiRepo.GetAllAsync(new QueryOptions<KoiFish>
            {
                Predicate = k => k.BreedingProcessId == breedingProcessId,
                IncludeProperties = new List<Expression<Func<KoiFish, object>>>
        {
                    k => k.Variety,
                    k => k.Pond,
                    k => k.BreedingProcess
        },
                Tracked = false
            });

            if (!koiList.Any())
            {
                return new List<KoiFishResponseDTO>(); 
            }

            var result = _mapper.Map<List<KoiFishResponseDTO>>(koiList);

            foreach (var koi in result)
                FormatSizeForResponse(koi);
            return result;
        }

        private void FormatSizeForResponse(KoiFishResponseDTO koi)
        {
            if (double.TryParse(koi.Size?.ToString(), out double cm))
            {
                var inch = CmToInch(cm);
                koi.Size = $"{Math.Round(inch, 1)} inch / {cm} cm";
            }
        }

        public static double CmToInch(double cm)
        {
            return cm / 2.54;
        }

    }
}



