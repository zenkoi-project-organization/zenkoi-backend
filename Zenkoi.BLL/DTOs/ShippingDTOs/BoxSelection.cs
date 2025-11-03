namespace Zenkoi.BLL.DTOs.ShippingDTOs
{
    public class BoxSelection
    {
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public int Quantity { get; set; }
        public decimal FeePerBox { get; set; }
        public decimal Subtotal { get; set; }
        public List<KoiInBox> KoiList { get; set; }
    }
}