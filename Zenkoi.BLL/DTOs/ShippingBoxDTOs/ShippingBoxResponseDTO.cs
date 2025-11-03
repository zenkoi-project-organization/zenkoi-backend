namespace Zenkoi.BLL.DTOs.ShippingBoxDTOs
{
    public class ShippingBoxResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int WeightCapacityLb { get; set; }
        public decimal Fee { get; set; }
        public int? MaxKoiCount { get; set; }
        public int? MaxKoiSizeInch { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ShippingBoxRuleResponseDTO> Rules { get; set; }
    }
}
