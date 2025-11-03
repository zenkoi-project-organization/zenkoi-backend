namespace Zenkoi.BLL.DTOs.ShippingDTOs
{
    public class ShippingBoxDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WeightCapacityLb { get; set; }
        public decimal Fee { get; set; }
        public int? MaxKoiCount { get; set; }
        public int? MaxKoiSizeInch { get; set; }
        public string Notes { get; set; }
    }
}