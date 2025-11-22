namespace Zenkoi.BLL.DTOs.PaymentDTOs
{
    public class PaymentCallbackRequestDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
