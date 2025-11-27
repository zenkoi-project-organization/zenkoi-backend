using System.ComponentModel.DataAnnotations;
using Net.payOS.Types;

namespace Zenkoi.BLL.DTOs.PayOSDTOs
{
    public class PayOSPaymentRequestDTO
    {
        [Required(ErrorMessage = "OrderCode không được để trống.")]
        public int OrderCode { get; set; }

        [Required(ErrorMessage = "Amount không được để trống.")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount phải lớn hơn 0.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Description không được để trống.")]
        [StringLength(255, ErrorMessage = "Description không được vượt quá 255 ký tự.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Items không được để trống.")]
        public List<ItemData> Items { get; set; } = new List<ItemData>();
        public int? ActualOrderId { get; set; }
    }
}
