using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.CustomerDTOs
{
    public class CustomerUpdateDTO
    {
        public string? ContactNumber { get; set; }

        public bool? IsActive { get; set; }
    }
}
