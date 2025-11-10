using Zenkoi.BLL.DTOs.OrderDTOs;
using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.BLL.Helpers;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;
using Zenkoi.DAL.Queries;
using Zenkoi.DAL.Repositories;
using Zenkoi.DAL.UnitOfWork;

namespace Zenkoi.BLL.Services.Implements
{
    public class ShippingFeeCalculationService : IShippingFeeCalculationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepoBase<CustomerAddress> _customerAddressRepo;
        private readonly IRepoBase<KoiFish> _koiFishRepo;
        private readonly IRepoBase<PacketFish> _packetFishRepo;
        private readonly IRepoBase<ShippingBox> _shippingBoxRepo;
        private readonly IRepoBase<ShippingDistance> _shippingDistanceRepo;
        private readonly IShippingCalculatorService _shippingCalculatorService;

        public ShippingFeeCalculationService(
            IUnitOfWork unitOfWork,
            IShippingCalculatorService shippingCalculatorService)
        {
            _unitOfWork = unitOfWork;
            _shippingCalculatorService = shippingCalculatorService;
            _customerAddressRepo = _unitOfWork.GetRepo<CustomerAddress>();
            _koiFishRepo = _unitOfWork.GetRepo<KoiFish>();
            _packetFishRepo = _unitOfWork.GetRepo<PacketFish>();
            _shippingBoxRepo = _unitOfWork.GetRepo<ShippingBox>();
            _shippingDistanceRepo = _unitOfWork.GetRepo<ShippingDistance>();
        }

        public async Task<ShippingFeeBreakdownDTO> CalculateShippingFeeAsync(CalculateShippingFeeRequestDTO request)
        {
            var items = request.Items.Select(i => new OrderItemDTO
            {
                KoiFishId = i.KoiFishId,
                PacketFishId = i.PacketFishId,
                Quantity = i.Quantity
            }).ToList();

            return await CalculateShippingFeeForOrderAsync(items, request.CustomerAddressId);
        }

        public async Task<ShippingFeeBreakdownDTO> CalculateShippingFeeForOrderAsync(List<OrderItemDTO> items, int customerAddressId)
        {
            var customerAddress = await _customerAddressRepo.GetByIdAsync(customerAddressId);
            if (customerAddress == null)
            {
                throw new ArgumentException("Customer address not found");
            }

            if (!customerAddress.DistanceFromFarmKm.HasValue)
            {
                if (!customerAddress.Latitude.HasValue || !customerAddress.Longitude.HasValue)
                {
                    throw new ArgumentException($"Customer address (ID: {customerAddressId}) does not have coordinates. Please update the address with latitude and longitude first.");
                }
                throw new ArgumentException($"Customer address (ID: {customerAddressId}) does not have distance calculated. Please call POST /api/CustomerAddress/{customerAddressId}/calculate-distance with farm coordinates to calculate distance first.");
            }

            var koiInputs = await ConvertOrderItemsToKoiInputsAsync(items);

            var shippingCalculation = await _shippingCalculatorService.CalculateShipping(
                new ShippingCalculationRequest
                {
                    KoiInputs = koiInputs
                }
            );

            var distanceFeeResult = await CalculateDistanceFeeAsync(customerAddress.DistanceFromFarmKm.Value);

            var totalBoxFee = shippingCalculation.TotalFee;
            var totalFishCount = shippingCalculation.TotalKoiCount;

            var totalBoxCount = shippingCalculation.Boxes.Sum(b => b.Quantity);

            return new ShippingFeeBreakdownDTO
            {
                BoxFee = totalBoxFee,
                ShippingBoxId = shippingCalculation.Boxes.FirstOrDefault()?.BoxId,
                ShippingBoxName = totalBoxCount > 1
                    ? $"{totalBoxCount} boxes"
                    : shippingCalculation.Boxes.FirstOrDefault()?.BoxName,
                Boxes = shippingCalculation.Boxes,
                DistanceFee = distanceFeeResult.Fee,
                ShippingDistanceId = distanceFeeResult.ShippingDistanceId,
                DistanceKm = customerAddress.DistanceFromFarmKm,
                TotalShippingFee = totalBoxFee + distanceFeeResult.Fee,
                Notes = $"Total fish count: {totalFishCount}, Boxes: {totalBoxCount}, Distance: {customerAddress.DistanceFromFarmKm:F2}km"
            };
        }

        private async Task<List<KoiSizeInput>> ConvertOrderItemsToKoiInputsAsync(List<OrderItemDTO> items)
        {
            var koiInputs = new List<KoiSizeInput>();

            foreach (var item in items)
            {
                if (item.KoiFishId.HasValue)
                {
                    var koiFish = await _koiFishRepo.GetByIdAsync(item.KoiFishId.Value);
                    if (koiFish == null)
                    {
                        throw new ArgumentException($"KoiFish with ID {item.KoiFishId} not found");
                    }

                    koiInputs.Add(new KoiSizeInput
                    {
//                       Size = koiFish.Size,
                        Quantity = item.Quantity
                    });
                }
                else if (item.PacketFishId.HasValue)
                {
                    var packetFish = await _packetFishRepo.GetByIdAsync(item.PacketFishId.Value);
                    if (packetFish == null)
                    {
                        throw new ArgumentException($"PacketFish with ID {item.PacketFishId} not found");
                    }

                    koiInputs.Add(new KoiSizeInput
                    {
                       // Size = packetFish.Size,
                        Quantity = packetFish.FishPerPacket * item.Quantity
                    });
                }
            }

            return koiInputs;
        }

        private async Task<(decimal Fee, int? ShippingBoxId, string? ShippingBoxName)> CalculateBoxFeeAsync_OLD(int totalFishCount, int maxFishSizeCm)
        {
            var queryOptions = new QueryOptions<ShippingBox>
            {
                Predicate = b => b.IsActive == true,
                OrderBy = q => q.OrderBy(b => b.Fee)
            };

            var shippingBoxes = await _shippingBoxRepo.GetAllAsync(queryOptions);

            int maxFishSizeInch = (int)Math.Ceiling(maxFishSizeCm / 2.54);

            foreach (var box in shippingBoxes)
            {
                bool meetsMaxCount = !box.MaxKoiCount.HasValue || totalFishCount <= box.MaxKoiCount.Value;
                bool meetsMaxSize = !box.MaxKoiSizeInch.HasValue || maxFishSizeInch <= box.MaxKoiSizeInch.Value;

                if (meetsMaxCount && meetsMaxSize)
                {
                    return (box.Fee, box.Id, box.Name);
                }
            }

            var largestBox = shippingBoxes.OrderByDescending(b => b.MaxKoiCount ?? int.MaxValue)
                                          .ThenByDescending(b => b.MaxKoiSizeInch ?? int.MaxValue)
                                          .FirstOrDefault();

            if (largestBox == null)
            {
                throw new InvalidOperationException("No shipping box available. Please configure shipping boxes first.");
            }

            return (largestBox.Fee, largestBox.Id, largestBox.Name);
        }

        private async Task<(decimal Fee, int? ShippingDistanceId)> CalculateDistanceFeeAsync(decimal distanceKm)
        {
            var queryOptions = new QueryOptions<ShippingDistance>
            {
                Predicate = d => d.IsActive == true &&
                                 d.MinDistanceKm <= (int)distanceKm &&
                                 d.MaxDistanceKm >= (int)distanceKm
            };

            var shippingDistance = await _shippingDistanceRepo.GetSingleAsync(queryOptions);

            if (shippingDistance == null)
            {
                var fallbackOptions = new QueryOptions<ShippingDistance>
                {
                    Predicate = d => d.IsActive == true,
                    OrderBy = q => q.OrderByDescending(d => d.MaxDistanceKm)
                };

                shippingDistance = await _shippingDistanceRepo.GetSingleAsync(fallbackOptions);

                if (shippingDistance == null)
                {
                    throw new InvalidOperationException("No shipping distance rule available. Please configure shipping distance rules first.");
                }
            }

            decimal totalFee = shippingDistance.BaseFee + (distanceKm * shippingDistance.PricePerKm);

            return (totalFee, shippingDistance.Id);
        }
    }
}
