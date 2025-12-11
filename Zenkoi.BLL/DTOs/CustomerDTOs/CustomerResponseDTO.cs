using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.CustomerDTOs
{
    public class CustomerResponseDTO
    {
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ContactNumber { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CustomerOrderSummaryDTO> RecentOrders { get; set; } = new List<CustomerOrderSummaryDTO>();
    }

    public class CustomerOrderSummaryDTO
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
