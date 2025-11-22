using System;

namespace Zenkoi.BLL.DTOs.PaymentTransactionDTOs
{
    public class PaymentTransactionFilterDTO
    {
        public int? UserId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? SearchKeyword { get; set; }
    }
}
