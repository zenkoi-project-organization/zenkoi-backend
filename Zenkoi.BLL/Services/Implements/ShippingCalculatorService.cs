using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Zenkoi.BLL.DTOs.ShippingDTOs;
using Zenkoi.BLL.Helpers;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.EF;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Services.Implements
{
    public class ShippingCalculatorService : IShippingCalculatorService
    {
        private readonly ZenKoiContext _context;
        private readonly ILogger<ShippingCalculatorService> _logger;

        public ShippingCalculatorService(
            ZenKoiContext context,
            ILogger<ShippingCalculatorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ShippingCalculationResult> CalculateShipping(ShippingCalculationRequest request)
        {
            _logger.LogInformation("Starting shipping calculation for {Count} koi groups",
                request.KoiInputs.Count);

            var result = new ShippingCalculationResult
            {
                Boxes = new List<BoxSelection>(),
                Warnings = new List<string>(),
                Suggestions = new List<string>()
            };

            if (request.KoiInputs == null || !request.KoiInputs.Any())
            {
                throw new ArgumentException("At least one koi size input is required");
            }

            var koiList = ExpandKoiInputs(request.KoiInputs);
            result.TotalKoiCount = koiList.Count;

            _logger.LogInformation("Expanded to {Count} individual koi", koiList.Count);
            foreach (var koi in koiList)
            {
                _logger.LogInformation("Koi: {SizeCm}cm ({SizeInch}inch), IsTosai: {IsTosai}",
                    koi.SizeCm, koi.SizeInch, koi.IsTosai);
            }

            var boxes = await _context.ShippingBoxes
                .Include(b => b.Rules.Where(r => r.IsActive))
                .Where(b => b.IsActive && b.MaxKoiCount.HasValue && b.MaxKoiCount.Value > 0)
                .OrderBy(b => b.Fee / b.MaxKoiCount.Value)
                .ToListAsync();

            if (!boxes.Any())
            {
                throw new InvalidOperationException("No shipping boxes available");
            }

            _logger.LogInformation("Loaded {Count} boxes", boxes.Count);

            var remainingKoi = new List<KoiItem>(koiList);
            var packingAttempts = 0;
            const int maxAttempts = 100;

            while (remainingKoi.Any() && packingAttempts < maxAttempts)
            {
                packingAttempts++;
                bool packed = false;

                _logger.LogInformation("Packing attempt #{Attempt}, remaining koi: {Count}",
                    packingAttempts, remainingKoi.Count);


                foreach (var box in boxes)
                {
                    var packedKoi = TryPackIntoBox(remainingKoi, box);

                    if (packedKoi.Any())
                    {
                        _logger.LogInformation("Packed {Count} koi into {BoxName} (${Fee})",
                            packedKoi.Count, box.Name, box.Fee);

                        result.Boxes.Add(new BoxSelection
                        {
                            BoxId = box.Id,
                            BoxName = box.Name,
                            Quantity = 1,
                            FeePerBox = box.Fee,
                            Subtotal = box.Fee,
                            KoiList = packedKoi.Select(k => new KoiInBox
                            {
                                SizeCm = k.SizeCm,
                                SizeInch = k.SizeInch
                            }).ToList()
                        });


                        foreach (var koi in packedKoi)
                        {
                            remainingKoi.Remove(koi);
                        }

                        packed = true;
                        break;
                    }
                }


                if (!packed)
                {
                    _logger.LogWarning("Unable to pack {Count} remaining koi into standard boxes",
                        remainingKoi.Count);

                    var extraLargeBox = await _context.ShippingBoxes
                        .FirstOrDefaultAsync(b => b.IsActive && !b.MaxKoiCount.HasValue);

                    if (extraLargeBox != null)
                    {
                        var maxKoiSizeInch = remainingKoi.Max(k => k.SizeInch);

                        if (extraLargeBox.MaxKoiSizeInch.HasValue && maxKoiSizeInch > extraLargeBox.MaxKoiSizeInch.Value)
                        {
                            throw new InvalidOperationException(
                                $"Có cá quá lớn ({maxKoiSizeInch:F2} inch) không thể vận chuyển. " +
                                $"Kích thước tối đa: {extraLargeBox.MaxKoiSizeInch.Value} inch. " +
                                "Vui lòng liên hệ quản lý trang trại."
                            );
                        }

                        result.Boxes.Add(new BoxSelection
                        {
                            BoxId = extraLargeBox.Id,
                            BoxName = extraLargeBox.Name,
                            Quantity = 1,
                            FeePerBox = extraLargeBox.Fee,
                            Subtotal = extraLargeBox.Fee,
                            KoiList = remainingKoi.Select(k => new KoiInBox
                            {
                                SizeCm = k.SizeCm,
                                SizeInch = k.SizeInch
                            }).ToList()
                        });

                        result.Warnings.Add(
                            $"{remainingKoi.Count} koi yêu cầu Extra Large Box - cần chấp thuận từ quản lý"
                        );

                        remainingKoi.Clear();
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Không thể đóng gói tất cả cá. Vui lòng liên hệ quản lý trang trại."
                        );
                    }
                }
            }

            result.Boxes = result.Boxes
                .GroupBy(b => new { b.BoxId, b.BoxName, b.FeePerBox })
                .Select(g => new BoxSelection
                {
                    BoxId = g.Key.BoxId,
                    BoxName = g.Key.BoxName,
                    Quantity = g.Count(),
                    FeePerBox = g.Key.FeePerBox,
                    Subtotal = g.Sum(b => b.Subtotal),
                    KoiList = g.SelectMany(b => b.KoiList).ToList()
                })
                .OrderBy(b => b.FeePerBox)
                .ToList();

            result.TotalFee = result.Boxes.Sum(b => b.Subtotal);

            AddSuggestions(result);

            _logger.LogInformation(
                "Calculation complete: {BoxCount} boxes ({Details}), Total: ${Total}",
                result.Boxes.Sum(b => b.Quantity),
                string.Join(", ", result.Boxes.Select(b => $"{b.Quantity}x {b.BoxName}")),
                result.TotalFee
            );

            return result;
        }

        private List<KoiItem> ExpandKoiInputs(List<KoiSizeInput> inputs)
        {
            var koiList = new List<KoiItem>();

            foreach (var input in inputs)
            {
                if (input.Quantity <= 0)
                    continue;

                var sizeCm = FishSizeHelper.GetSizeInCm(input.Size);

                for (int i = 0; i < input.Quantity; i++)
                {
                    koiList.Add(new KoiItem
                    {
                        SizeCm = sizeCm,
                        SizeInch = Math.Round(sizeCm / 2.54m, 2),
                        IsTosai = sizeCm <= 10 
                    });
                }
            }

            return koiList.OrderByDescending(k => k.SizeCm).ToList();
        }

        private List<KoiItem> TryPackIntoBox(
            List<KoiItem> availableKoi,
            ShippingBox box)
        {
            var packed = new List<KoiItem>();

            if (!box.MaxKoiCount.HasValue || !box.MaxKoiSizeInch.HasValue)
            {
                return packed;
            }

            _logger.LogDebug("Trying to pack into {BoxName} (max {MaxCount} koi, max {MaxSize} inch)",
                box.Name, box.MaxKoiCount, box.MaxKoiSizeInch);

            decimal maxSizeInch = box.MaxKoiSizeInch.Value;

            var suitableKoi = availableKoi
                .Where(k => k.SizeInch <= maxSizeInch)
                .ToList();

            _logger.LogDebug("Found {Count} koi that fit size limit of {MaxSize} inch",
                suitableKoi.Count, maxSizeInch);

            if (box.Name.Contains("Mini", StringComparison.OrdinalIgnoreCase))
            {
                suitableKoi = suitableKoi.Where(k => k.IsTosai).ToList();
                _logger.LogDebug("Mini Box: Filtered to {Count} tosai", suitableKoi.Count);
            }

            if (box.Rules != null && box.Rules.Any())
            {
                var activeRules = box.Rules
                    .Where(r => r.IsActive)
                    .OrderByDescending(r => r.Priority)
                    .ToList();

                foreach (var rule in activeRules)
                {
                    suitableKoi = ApplyRule(suitableKoi, rule);
                    if (!suitableKoi.Any())
                        break;
                }

                _logger.LogDebug("After applying rules: {Count} suitable koi", suitableKoi.Count);
            }

            packed = suitableKoi.Take(box.MaxKoiCount.Value).ToList();

            if (packed.Any())
            {
                _logger.LogInformation("Successfully packed {Count} koi into {BoxName}",
                    packed.Count, box.Name);
            }

            return packed;
        }

        private List<KoiItem> ApplyRule(List<KoiItem> koi, ShippingBoxRule rule)
        {
            _logger.LogDebug("Applying rule: {RuleType}", rule.RuleType);

            switch (rule.RuleType)
            {
                case ShippingRuleType.ByMaxLength:
                    if (rule.MaxLengthCm.HasValue)
                    {
                        koi = koi.Where(k => k.SizeCm <= rule.MaxLengthCm.Value).ToList();
                        _logger.LogDebug("Filtered by max length {Max}cm: {Count} koi remain",
                            rule.MaxLengthCm.Value, koi.Count);
                    }
                    if (rule.MinLengthCm.HasValue)
                    {
                        koi = koi.Where(k => k.SizeCm >= rule.MinLengthCm.Value).ToList();
                        _logger.LogDebug("Filtered by min length {Min}cm: {Count} koi remain",
                            rule.MinLengthCm.Value, koi.Count);
                    }
                    break;

                case ShippingRuleType.ByAge:
                    koi = koi.Where(k => k.IsTosai).ToList();
                    _logger.LogDebug("Filtered by age (tosai only): {Count} koi remain", koi.Count);
                    break;

                case ShippingRuleType.ByCount:
                    break;

                case ShippingRuleType.ByWeight:
                    break;
            }

            return koi;
        }

        private void AddSuggestions(ShippingCalculationResult result)
        {
            var totalBoxes = result.Boxes.Sum(b => b.Quantity);

            if (totalBoxes > 3)
            {
                result.Suggestions.Add(
                    "Xem xét gộp lô hàng để giảm số lượng box và tiết kiệm chi phí vận chuyển"
                );
            }

            var mediumBoxCount = result.Boxes
                .FirstOrDefault(b => b.BoxName.Contains("Medium"))?.Quantity ?? 0;

            if (mediumBoxCount >= 2)
            {
                result.Suggestions.Add(
                    "Xem xét dùng Large Box thay vì nhiều Medium Box để tiết kiệm"
                );
            }
        }

        public async Task<List<ShippingBoxDto>> GetAvailableBoxes()
        {
            return await _context.ShippingBoxes
                .Where(b => b.IsActive)
                .OrderBy(b => b.Fee)
                .Select(b => new ShippingBoxDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    WeightCapacityLb = b.WeightCapacityLb,
                    Fee = b.Fee,
                    MaxKoiCount = b.MaxKoiCount,
                    MaxKoiSizeInch = b.MaxKoiSizeInch,
                    Notes = b.Notes
                })
                .ToListAsync();
        }

        public async Task<ShippingBox> GetBoxById(int boxId)
        {
            return await _context.ShippingBoxes
                .Include(b => b.Rules.Where(r => r.IsActive))
                .FirstOrDefaultAsync(b => b.Id == boxId && b.IsActive);
        }

        private class KoiItem
        {
            public decimal SizeCm { get; set; }
            public decimal SizeInch { get; set; }
            public bool IsTosai { get; set; }
        }
    }
}
