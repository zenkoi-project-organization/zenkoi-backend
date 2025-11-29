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
            var queryOptions = new QueryOptions<ShippingBox>
            {
                Predicate = b => !b.IsDeleted
            };
            var boxes = await _shippingBoxRepo.GetAllAsync(queryOptions);

            var result = new List<ShippingBoxResponseDTO>();
            foreach (var box in boxes)
            {
                var dto = _mapper.Map<ShippingBoxResponseDTO>(box);

                var ruleQueryOptions = new QueryOptions<ShippingBoxRule>
                {
                    Predicate = r => r.ShippingBoxId == box.Id && r.IsActive == true
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
            if (box == null || box.IsDeleted)
            {
                throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển");
            }

            var dto = _mapper.Map<ShippingBoxResponseDTO>(box);

            var queryOptions = new QueryOptions<ShippingBoxRule>
            {
                Predicate = r => r.ShippingBoxId == id && r.IsActive == true
            };
            var rules = await _shippingBoxRuleRepo.GetAllAsync(queryOptions);
            dto.Rules = _mapper.Map<List<ShippingBoxRuleResponseDTO>>(rules);

            return dto;
        }

        public async Task<ShippingBoxResponseDTO> CreateAsync(ShippingBoxRequestDTO dto)
        {
            if (dto.WeightCapacityLb <= 0)
            {
                throw new ArgumentException("WeightCapacityLb must be greater than 0");
            }

            if (dto.Fee <= 0)
            {
                throw new ArgumentException("Fee must be greater than 0");
            }

            if (dto.MaxKoiCount.HasValue && dto.MaxKoiCount.Value <= 0)
            {
                throw new ArgumentException("MaxKoiCount must be greater than 0 if specified");
            }

            if (dto.MaxKoiSizeInch.HasValue && dto.MaxKoiSizeInch.Value <= 0)
            {
                throw new ArgumentException("MaxKoiSizeInch must be greater than 0 if specified");
            }

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

            if (dto.WeightCapacityLb <= 0)
            {
                throw new ArgumentException("WeightCapacityLb must be greater than 0");
            }

            if (dto.Fee <= 0)
            {
                throw new ArgumentException("Fee must be greater than 0");
            }

            if (dto.MaxKoiCount.HasValue && dto.MaxKoiCount.Value <= 0)
            {
                throw new ArgumentException("MaxKoiCount must be greater than 0 if specified");
            }

            if (dto.MaxKoiSizeInch.HasValue && dto.MaxKoiSizeInch.Value <= 0)
            {
                throw new ArgumentException("MaxKoiSizeInch must be greater than 0 if specified");
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

            box.IsDeleted = true;
            box.DeletedAt = DateTime.UtcNow;
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

                ValidateShippingBoxRule(dto.MaxCount, dto.MaxLengthCm, dto.MinLengthCm, dto.MaxWeightLb, dto.Priority);

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

        public async Task<bool> UpdateRuleAsync(int ruleId, ShippingBoxRuleUpdateDTO dto)
        {
            var rule = await _shippingBoxRuleRepo.GetByIdAsync(ruleId);
            if (rule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy quy tắc vận chuyển");
            }

            ValidateShippingBoxRule(dto.MaxCount, dto.MaxLengthCm, dto.MinLengthCm, dto.MaxWeightLb, dto.Priority);

            _mapper.Map(dto, rule);

            await _shippingBoxRuleRepo.UpdateAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private void ValidateShippingBoxRule(int? maxCount, int? maxLengthCm, int? minLengthCm, int? maxWeightLb, int priority)
        {
            if (maxCount.HasValue && maxCount.Value <= 0)
            {
                throw new ArgumentException("MaxCount must be greater than 0 if specified");
            }

            if (maxLengthCm.HasValue && maxLengthCm.Value <= 0)
            {
                throw new ArgumentException("MaxLengthCm must be greater than 0 if specified");
            }

            if (minLengthCm.HasValue && minLengthCm.Value <= 0)
            {
                throw new ArgumentException("MinLengthCm must be greater than 0 if specified");
            }

            if (maxWeightLb.HasValue && maxWeightLb.Value <= 0)
            {
                throw new ArgumentException("MaxWeightLb must be greater than 0 if specified");
            }

            if (minLengthCm.HasValue && maxLengthCm.HasValue && minLengthCm.Value > maxLengthCm.Value)
            {
                throw new ArgumentException("MinLengthCm cannot be greater than MaxLengthCm");
            }

            if (priority <= 0)
            {
                throw new ArgumentException("Priority must be greater than 0");
            }
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
            if (rule == null || !rule.IsActive)
            {
                throw new KeyNotFoundException("Không tìm thấy quy tắc vận chuyển");
            }

            return _mapper.Map<ShippingBoxRuleResponseDTO>(rule);
        }

        public async Task<List<ShippingBoxRuleResponseDTO>> GetRulesByBoxIdAsync(int boxId)
        {
            var queryOptions = new QueryOptions<ShippingBoxRule>
            {
                Predicate = r => r.ShippingBoxId == boxId && r.IsActive == true
            };
            var rules = await _shippingBoxRuleRepo.GetAllAsync(queryOptions);
            return _mapper.Map<List<ShippingBoxRuleResponseDTO>>(rules);
        }
    }
}
