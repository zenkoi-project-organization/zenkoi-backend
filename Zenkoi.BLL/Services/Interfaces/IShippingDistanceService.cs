using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenkoi.BLL.DTOs.ShippingDistanceDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IShippingDistanceService
    {
        Task<List<ShippingDistanceResponseDTO>> GetAllAsync();
        Task<ShippingDistanceResponseDTO> GetByIdAsync(int id);
        Task<ShippingDistanceResponseDTO> CreateAsync(ShippingDistanceRequestDTO dto);
        Task<bool> UpdateAsync(int id, ShippingDistanceRequestDTO dto);
        Task<bool> DeleteAsync(int id); 
    }
}
