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

            var boxes = await _context.ShippingBoxes
                .Include(b => b.Rules)
                .Where(b => b.IsActive && b.MaxKoiCount.HasValue)
                .OrderBy(b => b.Fee)
                .ToListAsync();

            if (!boxes.Any())
            {
                throw new InvalidOperationException("No shipping boxes available");
            }

            var remainingKoi = new List<KoiItem>(koiList);
            var packingAttempts = 0;
            const int maxAttempts = 100;

            while (remainingKoi.Any() && packingAttempts < maxAttempts)
            {
                packingAttempts++;
                bool packed = false;

                foreach (var box in boxes)
                {
                    var packedKoi = TryPackIntoBox(
                        remainingKoi,
                        box
                    );

                    if (packedKoi.Any())
                    {
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
                            $"{remainingKoi.Count} koi require Extra Large Box - farm manager approval needed"
                        );

                        remainingKoi.Clear();
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            "Unable to pack all koi. Please contact farm manager."
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
                "Calculation complete: {BoxCount} boxes, Total: ${Total}",
                result.Boxes.Sum(b => b.Quantity),
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
                        SizeInch = sizeCm / 2.54m,
                        IsTosai = sizeCm <= 15
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

            var suitableKoi = availableKoi
                .Where(k => k.SizeInch <= box.MaxKoiSizeInch.Value)
                .ToList();

            if (box.Name.Contains("Mini", StringComparison.OrdinalIgnoreCase))
            {
                suitableKoi = suitableKoi.Where(k => k.IsTosai).ToList();
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
            }

            packed = suitableKoi.Take(box.MaxKoiCount.Value).ToList();

            return packed;
        }

        private List<KoiItem> ApplyRule(List<KoiItem> koi, ShippingBoxRule rule)
        {
            switch (rule.RuleType)
            {
                case ShippingRuleType.ByMaxLength:
                    if (rule.MaxLengthCm.HasValue)
                    {
                        koi = koi.Where(k => k.SizeCm <= rule.MaxLengthCm.Value).ToList();
                    }
                    if (rule.MinLengthCm.HasValue)
                    {
                        koi = koi.Where(k => k.SizeCm >= rule.MinLengthCm.Value).ToList();
                    }
                    break;

                case ShippingRuleType.ByAge:
                    koi = koi.Where(k => k.IsTosai).ToList();
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
                    "Consider consolidating shipment to reduce number of boxes and save on shipping costs"
                );
            }

            var mediumBoxCount = result.Boxes
                .FirstOrDefault(b => b.BoxName.Contains("Medium"))?.Quantity ?? 0;

            if (mediumBoxCount >= 2)
            {
                result.Suggestions.Add(
                    "Consider using Large Boxes instead of multiple Medium Boxes for better value"
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
                .Include(b => b.Rules)
                .FirstOrDefaultAsync(b => b.Id == boxId);
        }

        private class KoiItem
        {
            public decimal SizeCm { get; set; }
            public decimal SizeInch { get; set; }
            public bool IsTosai { get; set; }
        }
    }
}