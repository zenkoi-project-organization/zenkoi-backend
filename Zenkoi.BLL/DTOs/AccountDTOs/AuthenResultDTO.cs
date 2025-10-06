namespace Zenkoi.BLL.DTOs.AccountDTOs
{
    public class AuthenResultDTO
    {
        public string Token { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
