namespace Zenkoi.BLL.DTOs.ShippingDTOs
{
    public class ShippingCalculationResult
    {
        public List<BoxSelection> Boxes { get; set; }
        public decimal TotalFee { get; set; }
        public int TotalKoiCount { get; set; }
        public List<string> Warnings { get; set; }
        public List<string> Suggestions { get; set; }
    }
}