using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.DashboardDTOs.SalesDashboardDTOs
{
    public class OrderSummaryDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

