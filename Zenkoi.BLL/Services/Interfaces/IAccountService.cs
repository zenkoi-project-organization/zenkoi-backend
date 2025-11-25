using Zenkoi.BLL.DTOs.AccountDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.DAL.Entities;

namespace Zenkoi.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        // Authentication & Authorization
        Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest);
        Task<AuthenResultDTO> GenerateTokenAsync(ApplicationUser user);
        Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO);
        Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult);
        Task<AuthenResultDTO> GenerateTokenFromRefreshTokenAsync(AuthenResultDTO authenResult);
        Task<AuthenResultDTO> SignInWithGoogleAsync(GoogleAuthDTO dto);

        // Email Verification
        Task<BaseResponse> SendOTPByEmailAsync(string email);
        Task<BaseResponse> ConfirmEmailByOTPAsync(string email, string otp);

        // Password Management
        Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto);
        Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto);

        // Staff Management
        Task<StaffAccountResponseDTO?> CreateStaffAccountAsync(StaffAccountRequestDTO dto);
        Task<ImportStaffResultDTO> ImportStaffAccountsFromExcelAsync(Stream fileStream);
        Task<StaffAccountResponseDTO?> UpdateStaffAccountAsync(int userId, StaffAccountUpdateDTO dto);
        Task<bool> ToggleBlockUserAsync(int userId);

        // Push Notification
        Task<bool> UpdatePushToken(int userId, UpdatePushTokenDTO dto);
        Task<IEnumerable<string>> GetStaffManagerTokensAsync();
    }
}
