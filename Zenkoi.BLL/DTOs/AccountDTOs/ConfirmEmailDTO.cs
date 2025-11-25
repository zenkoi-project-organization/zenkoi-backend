using System.ComponentModel.DataAnnotations;

namespace Zenkoi.BLL.DTOs.AccountDTOs
{
    public class ConfirmEmailDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Token là bắt buộc")]
        public string Token { get; set; }
    }
}
