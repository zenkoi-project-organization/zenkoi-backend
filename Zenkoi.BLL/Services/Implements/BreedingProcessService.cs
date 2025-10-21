using AutoMapper;
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
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Enums;
using System.Data;

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
            // Lấy BreedingProcessId của con cá
           // var fish = await _db.Set<KoiFish>()
           //    .AsNoTracking()
           //     .Select(f => new { f.Id, f.BreedingProcessId })
           //    .FirstOrDefaultAsync(f => f.Id == koiId );

            var _koifish = _unitOfWork.GetRepo<KoiFish>();
            var koifish = await _koifish.GetSingleAsync(new QueryOptions<KoiFish>
            {
                Predicate = KoiFish => KoiFish.Id == koiId,
            });

            if (koifish == null || koifish.BreedingProcessId == null)
                return (null, null); // founder hoặc chưa biết

            //  var bp = await _db.Set<BreedingProcess>()
            //      .AsNoTracking()
            //      .Select(b => new { b.Id, b.MaleKoiId, b.FemaleKoiId })
            //     .FirstOrDefaultAsync(b => b.Id == fish.BreedingProcessId.Value );'


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
            entity.Status = BreedingStatus.Spawned;
            entity.Result = BreedingResult.Unknown;
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
                b => b.Pond
        }
            });

            if (breeding == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy trình sinh sản");
            }

            return _mapper.Map<BreedingProcessResponseDTO>(breeding);
        }

        public async Task<PaginatedList<BreedingProcessResponseDTO>> GetAllBreedingProcess(int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<BreedingProcess>
            {
                IncludeProperties = new List<System.Linq.Expressions.Expression<Func<BreedingProcess, object>>>
        {
                b => b.MaleKoi,
                b => b.FemaleKoi,
                b => b.Pond
        }
            };

            var allBreeds = await _breedRepo.GetAllAsync(queryOptions);
            Console.WriteLine($"Số lượng BreedingProcess: {allBreeds.Count()}");


            var mappedList = allBreeds.Select(bp => _mapper.Map<BreedingProcessResponseDTO>(bp)).ToList();

            var count = mappedList.Count;
            var items = mappedList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<BreedingProcessResponseDTO>(items, count, pageIndex, pageSize);
        }
    }
}

