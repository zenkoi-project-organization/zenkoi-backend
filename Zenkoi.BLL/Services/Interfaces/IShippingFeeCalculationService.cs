using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.ShippingDTOs;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IShippingFeeCalculationService
    {
        Task<ShippingFeeBreakdownDTO> CalculateShippingFeeAsync(CalculateShippingFeeRequestDTO request);
        Task<ShippingFeeBreakdownDTO> CalculateShippingFeeForOrderAsync(List<OrderItemDTO> items, int customerAddressId);
    }
}
