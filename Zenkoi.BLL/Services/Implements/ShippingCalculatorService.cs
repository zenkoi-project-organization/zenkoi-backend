using Microsoft.EntityFrameworkCore;
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

        public ShippingCalculatorService(ZenKoiContext context)
        {
            _context = context;
        }

        public async Task<ShippingCalculationResult> CalculateShipping(ShippingCalculationRequest request)
        {         

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
                .Include(b => b.Rules.Where(r => r.IsActive))
                .Where(b => !b.IsDeleted && b.MaxKoiCount.HasValue && b.MaxKoiCount.Value > 0)
                .OrderBy(b => b.Fee / b.MaxKoiCount.Value)
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
          
                var bestCandidate = boxes
                    .Select(box =>
                    {
                        var packedKoi = TryPackIntoBox(remainingKoi, box);

                        if (!packedKoi.Any())
                            return null;

                        return new
                        {
                            Box = box,
                            PackedKoi = packedKoi,
                            PackedCount = packedKoi.Count,
                            CostPerFish = box.Fee / packedKoi.Count 
                        };
                    })
                    .Where(x => x != null)
                    .OrderBy(x => x.CostPerFish)          
                    .ThenByDescending(x => x.PackedCount)  
                    .FirstOrDefault();

                if (bestCandidate != null)
                {                  
                    result.Boxes.Add(new BoxSelection
                    {
                        BoxId = bestCandidate.Box.Id,
                        BoxName = bestCandidate.Box.Name,
                        Quantity = 1,
                        FeePerBox = bestCandidate.Box.Fee,
                        Subtotal = bestCandidate.Box.Fee,
                        KoiList = bestCandidate.PackedKoi.Select(k => new KoiInBox
                        {
                            SizeCm = k.SizeCm,
                            SizeInch = k.SizeInch
                        }).ToList()
                    });

                    foreach (var koi in bestCandidate.PackedKoi)
                    {
                        remainingKoi.Remove(koi);
                    }
                }
                else
                {
                    var extraLargeBox = await _context.ShippingBoxes
                        .FirstOrDefaultAsync(b => !b.IsDeleted && !b.MaxKoiCount.HasValue);

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
                        IsTosai = sizeCm >= 10 && sizeCm <= 25 
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
        

            decimal maxSizeInch = box.MaxKoiSizeInch.Value;

            var suitableKoi = availableKoi
                .Where(k => k.SizeInch <= maxSizeInch)
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
                .Where(b => !b.IsDeleted)
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
                .FirstOrDefaultAsync(b => b.Id == boxId && !b.IsDeleted);
        }

        private class KoiItem
        {
            public decimal SizeCm { get; set; }
            public decimal SizeInch { get; set; }
            public bool IsTosai { get; set; }
        }
    }
}
