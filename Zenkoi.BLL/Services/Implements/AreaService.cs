using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.AreaDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class AreaService : IAreaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Area> _areaRepo;
        public AreaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _areaRepo = _unitOfWork.GetRepo<Area>();
        }
        public async Task<PaginatedList<AreaResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var areas = await _areaRepo.GetAll();

            var mappedList = _mapper.Map<List<AreaResponseDTO>>(areas);

            var totalCount = mappedList.Count;
            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<AreaResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<AreaResponseDTO?> GetByIdAsync(int id)
        {
            var area = await _areaRepo.GetByIdAsync(id);
            if (area == null)
            {
                throw new KeyNotFoundException("không tìm thấy ví trí");
            }
            return _mapper.Map<AreaResponseDTO>(area);
        }

        public async Task<AreaResponseDTO> CreateAsync(AreaRequestDTO dto)
        {
            
            var entity = _mapper.Map<Area>(dto);
            await _areaRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AreaResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, AreaRequestDTO dto)
        {
            var area = await _areaRepo.GetByIdAsync(id);
            if (area == null) if (area == null)
             {
                    throw new KeyNotFoundException("không tìm thấy ví trí");
            }
            _mapper.Map(dto, area);
            _areaRepo.UpdateAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _areaRepo.GetByIdAsync(id);
            if (area == null)
            {
                throw new KeyNotFoundException("không tìm thấy ví trí");
            }

            _areaRepo.DeleteAsync(area);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}