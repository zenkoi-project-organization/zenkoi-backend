using System.ComponentModel.DataAnnotations;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.DTOs.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }

        [MaxLength(2000)]
        public string? Note { get; set; }
    }
}
