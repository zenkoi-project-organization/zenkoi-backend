using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
        
        public string? Note { get; set; }
    }
}
