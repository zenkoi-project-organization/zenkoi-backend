using System;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class OrderFilterRequestDTO
    {
        public string? Search { get; set; }
        public OrderStatus? Status { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public decimal? MinTotalAmount { get; set; }
        public decimal? MaxTotalAmount { get; set; }
        public bool? HasPromotion { get; set; }
        public string? OrderNumber { get; set; }
    }
}