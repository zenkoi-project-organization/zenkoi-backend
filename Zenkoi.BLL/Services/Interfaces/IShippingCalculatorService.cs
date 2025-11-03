using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IShippingCalculatorService
    {
        Task<ShippingCalculationResult> CalculateShipping(ShippingCalculationRequest request);
        Task<List<ShippingBoxDto>> GetAvailableBoxes();
        Task<ShippingBox> GetBoxById(int boxId);
    }
}