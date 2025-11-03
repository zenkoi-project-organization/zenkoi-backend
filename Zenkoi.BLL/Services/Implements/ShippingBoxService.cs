using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zenkoi.BLL.DTOs.ShippingBoxDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;
using Zenkoi.DAL.Queries;

namespace Zenkoi.BLL.Services.Implements
{
    public class ShippingBoxService : IShippingBoxService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<ShippingBox> _shippingBoxRepo;
        private readonly IRepoBase<ShippingBoxRule> _shippingBoxRuleRepo;

        public ShippingBoxService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shippingBoxRepo = _unitOfWork.GetRepo<ShippingBox>();
            _shippingBoxRuleRepo = _unitOfWork.GetRepo<ShippingBoxRule>();
        }

        public async Task<List<ShippingBoxResponseDTO>> GetAllAsync()
        {
            var queryOptions = new QueryOptions<ShippingBox>();
            var boxes = await _shippingBoxRepo.GetAllAsync(queryOptions);

            var result = new List<ShippingBoxResponseDTO>();
            foreach (var box in boxes)
            {
                var dto = _mapper.Map<ShippingBoxResponseDTO>(box);

                var ruleQueryOptions = new QueryOptions<ShippingBoxRule>
                {
                    Predicate = r => r.ShippingBoxId == box.Id
                };
                var rules = await _shippingBoxRuleRepo.GetAllAsync(ruleQueryOptions);
                dto.Rules = _mapper.Map<List<ShippingBoxRuleResponseDTO>>(rules);
                result.Add(dto);
            }

            return result;
        }

        public async Task<ShippingBoxResponseDTO> GetByIdAsync(int id)
        {
            var box = await _shippingBoxRepo.GetByIdAsync(id);
            if (box == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển");
            }

            var dto = _mapper.Map<ShippingBoxResponseDTO>(box);

            var queryOptions = new QueryOptions<ShippingBoxRule>
            {
                Predicate = r => r.ShippingBoxId == id
            };
            var rules = await _shippingBoxRuleRepo.GetAllAsync(queryOptions);
            dto.Rules = _mapper.Map<List<ShippingBoxRuleResponseDTO>>(rules);

            return dto;
        }

        public async Task<ShippingBoxResponseDTO> CreateAsync(ShippingBoxRequestDTO dto)
        {
            var entity = _mapper.Map<ShippingBox>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _shippingBoxRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ShippingBoxResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ShippingBoxRequestDTO dto)
        {
            var box = await _shippingBoxRepo.GetByIdAsync(id);
            if (box == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển");
            }

            _mapper.Map(dto, box);
            box.UpdatedAt = DateTime.UtcNow;

            await _shippingBoxRepo.UpdateAsync(box);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var box = await _shippingBoxRepo.GetByIdAsync(id);
            if (box == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển");
            }

            box.IsActive = false;
            box.UpdatedAt = DateTime.UtcNow;

            await _shippingBoxRepo.UpdateAsync(box);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ShippingBoxRuleResponseDTO> AddRuleAsync(ShippingBoxRuleRequestDTO dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var box = await _shippingBoxRepo.GetByIdAsync(dto.ShippingBoxId);
                if (box == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển");
                }

                var entity = _mapper.Map<ShippingBoxRule>(dto);
                entity.CreatedAt = DateTime.UtcNow;

                await _shippingBoxRuleRepo.CreateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<ShippingBoxRuleResponseDTO>(entity);
            }
            catch
            {
                await _unitOfWork.RollBackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateRuleAsync(int ruleId, ShippingBoxRuleRequestDTO dto)
        {
            var rule = await _shippingBoxRuleRepo.GetByIdAsync(ruleId);
            if (rule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy tắc vận chuyển");
            }

            _mapper.Map(dto, rule);

            await _shippingBoxRuleRepo.UpdateAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRuleAsync(int ruleId)
        {
            var rule = await _shippingBoxRuleRepo.GetByIdAsync(ruleId);
            if (rule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy tắc vận chuyển");
            }

            rule.IsActive = false;

            await _shippingBoxRuleRepo.UpdateAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<ShippingBoxRuleResponseDTO> GetRuleByIdAsync(int ruleId)
        {
            var rule = await _shippingBoxRuleRepo.GetByIdAsync(ruleId);
            if (rule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy tắc vận chuyển");
            }

            return _mapper.Map<ShippingBoxRuleResponseDTO>(rule);
        }

        public async Task<List<ShippingBoxRuleResponseDTO>> GetRulesByBoxIdAsync(int boxId)
        {
            var queryOptions = new QueryOptions<ShippingBoxRule>
            {
                Predicate = r => r.ShippingBoxId == boxId
            };
            var rules = await _shippingBoxRuleRepo.GetAllAsync(queryOptions);
            return _mapper.Map<List<ShippingBoxRuleResponseDTO>>(rules);
        }
    }
}
