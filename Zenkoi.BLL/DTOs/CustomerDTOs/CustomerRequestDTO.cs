using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CustomerDTOs
{
    public class CustomerRequestDTO
    {
        [Required]
        public int ApplicationUserId { get; set; }

        public string? ContactNumber { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
