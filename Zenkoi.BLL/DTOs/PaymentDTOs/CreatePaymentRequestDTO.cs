namespace Zenkoi.BLL.DTOs.PaymentDTOs
{
    public class CreatePaymentRequestDTO
    {
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = "VnPay"; // PayOS or VnPay
    }
}
