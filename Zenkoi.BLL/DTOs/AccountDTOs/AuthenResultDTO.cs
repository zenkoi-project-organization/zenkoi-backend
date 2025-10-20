namespace Zenkoi.BLL.DTOs.AccountDTOs
{
    public class AuthenResultDTO
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
