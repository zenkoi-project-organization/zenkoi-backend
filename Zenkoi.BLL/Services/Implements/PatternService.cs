using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.FilterDTOs;
using Zenkoi.BLL.DTOs.PatternDTOs;
using Zenkoi.BLL.DTOs.VarietyDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Paging;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class PatternService : IPatternService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<Pattern> _patternRepo;
        private readonly IRepoBase<Variety> _varietyRepo;
        private readonly IRepoBase<VarietyPattern> _varietyPatternRepo;

        public PatternService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _patternRepo = _unitOfWork.GetRepo<Pattern>();
            _varietyPatternRepo = _unitOfWork.GetRepo<VarietyPattern>();
            _varietyRepo = _unitOfWork.GetRepo<Variety>();
        }

        public async Task<PaginatedList<PatternResponseDTO>> GetAllAsync(int pageIndex = 1, int pageSize = 10)
        {
            var queryOptions = new QueryOptions<Pattern>();

            var patterns = await _patternRepo.GetAllAsync(queryOptions);

            var mappedList = _mapper.Map<List<PatternResponseDTO>>(patterns);

            var totalCount = mappedList.Count;

            var pagedItems = mappedList
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PatternResponseDTO>(pagedItems, totalCount, pageIndex, pageSize);
        }

        public async Task<PatternResponseDTO?> GetByIdAsync(int id)
        {
            var pattern = await _patternRepo.GetByIdAsync(id);
            return _mapper.Map<PatternResponseDTO>(pattern);
        }

        public async Task<PatternResponseDTO> CreateAsync(PatternRequestDTO dto)
        {
            var entity = _mapper.Map<Pattern>(dto);

            await _patternRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatternResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, PatternRequestDTO dto)
        {
            var pattern = await _patternRepo.GetByIdAsync(id);
            if (pattern == null) return false;

            _mapper.Map(dto, pattern);

            await _patternRepo.UpdateAsync(pattern);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pattern = await _patternRepo.GetByIdAsync(id);
            if (pattern == null) return false;

            await _patternRepo.DeleteAsync(pattern);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AssignPatternToVarietyAsync(int varietyId, int patternId)
        {
            // Kiểm tra Variety tồn tại
            var variety = await _varietyRepo.GetByIdAsync(varietyId);
            if (variety == null)
                throw new ArgumentException($"Variety with id {varietyId} not found.");

            // Kiểm tra Pattern tồn tại
            var pattern = await _patternRepo.GetByIdAsync(patternId);
            if (pattern == null)
                throw new ArgumentException($"Pattern with id {patternId} not found.");

            // Kiểm tra liên kết đã tồn tại chưa
            var options = new QueryOptions<VarietyPattern>
            {
                Predicate = x => x.VarietyId == varietyId && x.PatternId == patternId,
                Tracked = false
            };

            var existingLinks = await _varietyPatternRepo.GetAllAsync(options);
            if (existingLinks.Any())
            {
                // Đã gán rồi thì coi như thành công, không làm gì thêm
                return true;
            }

            // Tạo liên kết mới
            var vp = new VarietyPattern
            {
                VarietyId = varietyId,
                PatternId = patternId
            };

            await _varietyPatternRepo.CreateAsync(vp);
            return await _unitOfWork.SaveAsync();

        }

        public async Task<bool> RemovePatternFromVarietyAsync(int varietyId, int patternId)
        {
            var options = new QueryOptions<VarietyPattern>
            {
                Predicate = x => x.VarietyId == varietyId && x.PatternId == patternId
            };

            var links = await _varietyPatternRepo.GetAllAsync(options);
            var link = links.FirstOrDefault();
            if (link == null) return false;

            await _varietyPatternRepo.DeleteAsync(link);
     

            return await _unitOfWork.SaveAsync();
        }

        public async Task<PaginatedList<PatternResponseDTO>> GetPatternsByVarietyAsync(
            int varietyId,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var options = new QueryOptions<VarietyPattern>
            {
                Predicate = x => x.VarietyId == varietyId,
                Tracked = false,
                IncludeProperties = { x => x.Pattern }
            };

            var links = await _varietyPatternRepo.GetAllAsync(options);

            var patterns = links
                .Where(x => x.Pattern != null)
                .Select(x => x.Pattern!)
                .Distinct()
                .ToList();

            var mapped = _mapper.Map<List<PatternResponseDTO>>(patterns);

            var totalCount = mapped.Count;

            var pagedItems = mapped
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<PatternResponseDTO>(
                pagedItems, totalCount, pageIndex, pageSize);
        }
        public async Task<PaginatedList<VarietyResponseDTO>> GetVarietiesByPatternAsync(
       int patternId,
       int pageIndex = 1,
       int pageSize = 10)
        {
            var options = new QueryOptions<VarietyPattern>
            {
                Predicate = x => x.PatternId == patternId,
                Tracked = false,
                IncludeProperties = { x => x.Variety }
            };

            var links = await _varietyPatternRepo.GetAllAsync(options);

            var varieties = links
                .Where(x => x.Variety != null)
                .Select(x => x.Variety!)
                .Distinct()
                .ToList();

            var mapped = _mapper.Map<List<VarietyResponseDTO>>(varieties);

            var totalCount = mapped.Count;

            var pagedItems = mapped
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<VarietyResponseDTO>(
                pagedItems, totalCount, pageIndex, pageSize);
        }

    }
}
