namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxRequestDTO
    {
        public string Name { get; set; }
        public int WeightCapacityLb { get; set; }
        public decimal Fee { get; set; }
        public int? MaxKoiCount { get; set; }
        public int? MaxKoiSizeInch { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
