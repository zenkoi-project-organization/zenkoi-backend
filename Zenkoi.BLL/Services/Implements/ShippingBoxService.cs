using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zenkoi.BLL.DTOs.ShippingBoxDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
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
            if (dto.Fee <= 0)
            {
                throw new ArgumentException("Fee must be greater than 0");
            }
            else if (dto.Fee < 0)
            {
                throw new ArgumentException("Phí vận chuyển không được âm");
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


            if (dto.Fee <= 0)
            {
                throw new ArgumentException("Fee must be greater than 0");
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

                var existingRule = await _shippingBoxRuleRepo.GetSingleAsync(new QueryOptions<ShippingBoxRule>
                {
                    Predicate = r => r.ShippingBoxId == dto.ShippingBoxId
                                  && r.RuleType == dto.RuleType
                                  && r.IsActive
                });

                if (existingRule != null)
                {
                    throw new ArgumentException($"Hộp '{box.Name}' đã có quy tắc '{dto.RuleType}'. Vui lòng cập nhật quy tắc hiện tại thay vì tạo mới.");
                }

                ValidateShippingBoxRule(dto.RuleType, dto.MaxCount, dto.MaxLengthCm, dto.MinLengthCm, dto.MaxWeightLb, dto.Priority, box.MaxKoiSizeInch);

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

            var box = await _shippingBoxRepo.GetByIdAsync(rule.ShippingBoxId);
            if (box == null)
            {
                throw new KeyNotFoundException("Không tìm thấy hộp vận chuyển liên kết");
            }

            if (dto.RuleType != rule.RuleType)
            {
                var existingRule = await _shippingBoxRuleRepo.GetSingleAsync(new QueryOptions<ShippingBoxRule>
                {
                    Predicate = r => r.ShippingBoxId == rule.ShippingBoxId
                                  && r.RuleType == dto.RuleType
                                  && r.IsActive
                                  && r.Id != ruleId
                });

                if (existingRule != null)
                {
                    throw new ArgumentException($"Hộp '{box.Name}' đã có quy tắc '{dto.RuleType}'. Không thể thay đổi thành loại trùng.");
                }
            }

            ValidateShippingBoxRule(dto.RuleType, dto.MaxCount, dto.MaxLengthCm, dto.MinLengthCm, dto.MaxWeightLb, dto.Priority, box.MaxKoiSizeInch);

            _mapper.Map(dto, rule);

            await _shippingBoxRuleRepo.UpdateAsync(rule);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private void ValidateShippingBoxRule(ShippingRuleType ruleType, int? maxCount, int? maxLengthCm, int? minLengthCm, int? maxWeightLb, int priority, int? boxMaxKoiSizeInch)
        {
            if (priority <= 0)
            {
                throw new ArgumentException("Độ ưu tiên phải lớn hơn 0");
            }

            if (minLengthCm.HasValue && maxLengthCm.HasValue && minLengthCm.Value >= maxLengthCm.Value)
            {
                throw new ArgumentException($"Chiều dài tối thiểu ({minLengthCm}cm) phải nhỏ hơn chiều dài tối đa ({maxLengthCm}cm)");
            }

            if (boxMaxKoiSizeInch.HasValue)
            {
                decimal boxMaxSizeCm = boxMaxKoiSizeInch.Value * 2.54m;

                if (minLengthCm.HasValue && minLengthCm.Value >= boxMaxSizeCm)
                {
                    throw new ArgumentException(
                        $"HỘP MA PHÁT HIỆN: MinLengthCm ({minLengthCm}cm) KHÔNG được lớn hơn hoặc bằng " +
                        $"sức chứa vật lý của hộp ({boxMaxSizeCm:F1}cm = {boxMaxKoiSizeInch}inch). " +
                        $"Hậu quả: Không con cá nào vào được hộp này! " +
                        $"Vui lòng nhập MinLengthCm < {boxMaxSizeCm:F1}cm"
                    );
                }

                if (maxLengthCm.HasValue && maxLengthCm.Value > boxMaxSizeCm)
                {
                    Console.WriteLine($"WARNING: MaxLengthCm ({maxLengthCm}cm) vượt quá sức chứa hộp ({boxMaxSizeCm:F1}cm)");
                }
            }

            switch (ruleType)
            {
                case ShippingRuleType.ByMaxLength:
                    if (!minLengthCm.HasValue && !maxLengthCm.HasValue)
                    {
                        throw new ArgumentException("Quy tắc ByMaxLength cần ít nhất một trong MinLengthCm hoặc MaxLengthCm");
                    }
                    break;

                case ShippingRuleType.ByAge:
                    if (minLengthCm.HasValue || maxLengthCm.HasValue)
                    {
                        throw new ArgumentException("Quy tắc ByAge không nên có MinLengthCm hoặc MaxLengthCm (chỉ lọc theo tuổi cá)");
                    }
                    break;

                case ShippingRuleType.ByCount:
                    if (!maxCount.HasValue || maxCount.Value <= 0)
                    {
                        throw new ArgumentException("Quy tắc ByCount yêu cầu MaxCount > 0");
                    }
                    break;

                case ShippingRuleType.ByWeight:
                    if (!maxWeightLb.HasValue || maxWeightLb.Value <= 0)
                    {
                        throw new ArgumentException("Quy tắc ByWeight yêu cầu MaxWeightLb > 0");
                    }
                    break;
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
