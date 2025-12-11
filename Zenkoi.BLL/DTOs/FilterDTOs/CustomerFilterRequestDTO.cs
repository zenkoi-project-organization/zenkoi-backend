using System;

namespace Zenkoi.BLL.DTOs.FilterDTOs
{
    public class CustomerFilterRequestDTO
    {
        public string? Search { get; set; }
        public decimal? MinTotalSpent { get; set; }
        public decimal? MaxTotalSpent { get; set; }
        public int? MinTotalOrders { get; set; }
        public int? MaxTotalOrders { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string? ContactNumber { get; set; }
    }
}
