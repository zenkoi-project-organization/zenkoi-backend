using Zenkoi.BLL.DTOs.ShippingBoxDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IShippingBoxService
    {
        Task<List<ShippingBoxResponseDTO>> GetAllAsync();
        Task<ShippingBoxResponseDTO> GetByIdAsync(int id);
        Task<ShippingBoxResponseDTO> CreateAsync(ShippingBoxRequestDTO dto);
        Task<bool> UpdateAsync(int id, ShippingBoxRequestDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<ShippingBoxRuleResponseDTO> AddRuleAsync(ShippingBoxRuleRequestDTO dto);
        Task<bool> UpdateRuleAsync(int ruleId, ShippingBoxRuleRequestDTO dto);
        Task<bool> DeleteRuleAsync(int ruleId);
        Task<ShippingBoxRuleResponseDTO> GetRuleByIdAsync(int ruleId);
        Task<List<ShippingBoxRuleResponseDTO>> GetRulesByBoxIdAsync(int boxId);
    }
}
