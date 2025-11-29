using AutoMapper;
using Zenkoi.BLL.DTOs.ShippingDistanceDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class ShippingDistanceService : IShippingDistanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepoBase<ShippingDistance> _shippingDistanceRepo;

        public ShippingDistanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _shippingDistanceRepo = _unitOfWork.GetRepo<ShippingDistance>();
        }

        public async Task<List<ShippingDistanceResponseDTO>> GetAllAsync()
        {
            var queryOptions = new QueryOptions<ShippingDistance>();
            var distances = await _shippingDistanceRepo.GetAllAsync(queryOptions);
            return _mapper.Map<List<ShippingDistanceResponseDTO>>(distances);
        }

        public async Task<ShippingDistanceResponseDTO> GetByIdAsync(int id)
        {
            var distance = await _shippingDistanceRepo.GetByIdAsync(id);
            if (distance == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khoảng cách vận chuyển");
            }

            return _mapper.Map<ShippingDistanceResponseDTO>(distance);
        }

        public async Task<ShippingDistanceResponseDTO> CreateAsync(ShippingDistanceRequestDTO dto)
        {
            var entity = _mapper.Map<ShippingDistance>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            await _shippingDistanceRepo.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ShippingDistanceResponseDTO>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ShippingDistanceRequestDTO dto)
        {
            var distance = await _shippingDistanceRepo.GetByIdAsync(id);
            if (distance == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khoảng cách vận chuyển");
            }

            _mapper.Map(dto, distance);
            distance.UpdatedAt = DateTime.UtcNow;

            await _shippingDistanceRepo.UpdateAsync(distance);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var distance = await _shippingDistanceRepo.GetByIdAsync(id);
            if (distance == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khoảng cách vận chuyển");
            }

            distance.IsDeleted = true;
            distance.DeletedAt = DateTime.UtcNow;
            distance.UpdatedAt = DateTime.UtcNow;

            await _shippingDistanceRepo.UpdateAsync(distance);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
