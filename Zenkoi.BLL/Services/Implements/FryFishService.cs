using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FryFishDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Enums;
using Zenkoi.BLL.DTOs.EggBatchDTOs;
using Zenkoi.DAL.Paging;
using Zenkoi.BLL.Services.Interfaces;
namespace Zenkoi.BLL.Services.Implements
{
    public class FryFishService : IFryFishService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<FryFish> _fryFishRepo;
        public FryFishService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _fryFishRepo = _unitOfWork.GetRepo<FryFish>();
        }
        public async Task<FryFishResponseDTO> CreateAsync(FryFishRequestDTO dto)
        {
            var _breedRepo = _unitOfWork.GetRepo<BreedingProcess>();
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var _eggRepo = _unitOfWork.GetRepo<EggBatch>();
            var breeding = await _breedRepo.GetSingleAsync(new QueryOptions<BreedingProcess> {
                Predicate = b => b.Id == dto.BreedingProcessId ,
            IncludeProperties = new List<Expression<Func<BreedingProcess, object>>> { 
                e => e.Batch,
                f => f.FryFish
            }
            });
            if (breeding == null)
            {
                throw new KeyNotFoundException("không tìm thấy quy trình sinh sản");
            }
            if(breeding.Batch == null )
            {
                throw new Exception("Quy trình sinh sản chưa có ấp trứng nên không thể tạo nuôi cá bột");
            }
            if(!breeding.Batch.Status.Equals(EggBatchStatus.Success))
            {
                throw new Exception("Ấp trứng chưa hoàn thành nên không thể tạo nuôi cá bột");
            }
            if(breeding.FryFish != null)
            {
                throw new Exception("Quy trình sinh sản đã có nuôi cá bột");
            }


            var pond = await _pondRepo.GetByIdAsync(dto.PondId);
            if (pond == null)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            if (!pond.PondStatus.Equals(PondStatus.Empty))
            {
                throw new Exception("hiện tại hồ bạn chọn không trống");
            }
            var eggBatch = await _eggRepo.GetByIdAsync(breeding.Batch.Id);
            var eggPond = await _pondRepo.GetByIdAsync(breeding.Batch.PondId);
            
            
            // chuyển hồ
            eggBatch.PondId = null;
            pond.PondStatus = PondStatus.Active;
            eggPond.PondStatus = PondStatus.Empty;
            var fryFish = _mapper.Map<FryFish>(dto);
            fryFish.InitialCount = eggBatch.TotalHatchedEggs;
            fryFish.Status = FryFishStatus.Hatched;
            fryFish.StartDate = DateTime.Now;
            await _pondRepo.UpdateAsync(eggPond);
            await _pondRepo.UpdateAsync(pond);
            await _eggRepo.UpdateAsync(eggBatch);
            await _fryFishRepo.CreateAsync(fryFish);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<FryFishResponseDTO>(fryFish);
        }

        public async  Task<bool> DeleteAsync(int id)
        {
            var fryfish = await _fryFishRepo.GetByIdAsync(id);
            if (fryfish == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lô trứng cần xóa");
            }

            await _fryFishRepo.DeleteAsync(fryfish);

            return await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<FryFishResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var fryfish = await _fryFishRepo.GetAllAsync(new QueryOptions<FryFish>
            {
                IncludeProperties = new List<Expression<Func<FryFish, object>>>
                {
                    p => p.Pond
                }
            });

            var mappedList = _mapper.Map<List<FryFishResponseDTO>>(fryfish);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<FryFishResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<FryFishResponseDTO?> GetByIdAsync(int id)
        {
            var fryFish = await _fryFishRepo.GetSingleAsync(new QueryOptions<FryFish>
            {
                Predicate = e => e.Id == id,
                IncludeProperties = new List<Expression<Func<FryFish, object>>>
                {
                    p => p.Pond
                }
            });
            if (fryFish == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
            }
            return _mapper.Map<FryFishResponseDTO?>(fryFish);
    }

        public async  Task<bool> UpdateAsync(int id, FryFishUpdateRequestDTO dto)
        {
            var _pondRepo = _unitOfWork.GetRepo<Pond>();
            var pond = await _pondRepo.CheckExistAsync(dto.PondId);
            if (!pond)
            {
                throw new KeyNotFoundException("không tìm thấy hồ");
            }
            var fryFish = await _fryFishRepo.GetByIdAsync(id);
            if(fryFish == null)
            {
                throw new KeyNotFoundException(" không tìm thấy bầy cá");
            }
            _mapper.Map(dto, fryFish);
            await _fryFishRepo.UpdateAsync(fryFish);
            return  await _unitOfWork.SaveAsync();
        }
    }
}
