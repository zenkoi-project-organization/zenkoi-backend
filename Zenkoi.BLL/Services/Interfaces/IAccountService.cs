using Zenkoi.BLL.DTOs.AccountDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
        Task<AuthenResultDTO> SignInAsync(AuthenDTO authenDTO);
        Task<AuthenResultDTO> GenerateTokenAsync(ApplicationUser user);
        Task<BaseResponse> SendEmailConfirmation(ApplicationUser user);
        Task<BaseResponse> SendOTP2FA(ApplicationUser user, string password);
        Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO);
        Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult);
        Task<AuthenResultDTO> GenerateTokenFromRefreshTokenAsync(AuthenResultDTO authenResult);
        Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
        Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);
        Task<BaseResponse> SendOTPByEmailAsync(string email);
		Task<bool> VerifyOTPByEmailAsync(string email, string code);
        Task<AuthenResultDTO> SignInWithGoogleAsync(GoogleAuthDTO dto);
        Task<StaffAccountResponseDTO?> CreateStaffAccountAsync(StaffAccountRequestDTO dto);
        Task<ImportStaffResultDTO> ImportStaffAccountsFromExcelAsync(Stream fileStream);
        Task<StaffAccountResponseDTO?> UpdateStaffAccountAsync(int userId, StaffAccountUpdateDTO dto);
        Task<bool> ToggleBlockUserAsync(int userId);

        Task<bool> UpdatePushToken(int userId, UpdatePushTokenDTO dto);
        Task<IEnumerable<string>> GetStaffManagerTokensAsync();
    }
}
