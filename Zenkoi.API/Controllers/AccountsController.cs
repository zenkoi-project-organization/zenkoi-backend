using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Zenkoi.BLL.DTOs.AccountDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseAPIController
    {
        private readonly IAccountService _accountService;
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        public AccountsController(IAccountService accountService, IIdentityService identityService, IConfiguration configuration)
        {
            _accountService = accountService;
            _identityService = identityService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] AccountCreateRequestDTO accountCreate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ModelInvalid();
                }

                if (!accountCreate.Password.Equals(accountCreate.ConfirmPassword))
                {
                    ModelState.AddModelError("ConfirmPassword", "Nhắc lại mật khẩu không khớp với mật khẩu");
                    return ModelInvalid();
                }

                var userEmail = await _identityService.GetByEmailAsync(accountCreate.Email);
                if (userEmail != null)
                {
                    ModelState.AddModelError("EmailAddress", "Email đã tồn tại trong hệ thống. Vui lòng thử một email khác.");
                    return ModelInvalid();
                }

                var userName = await _identityService.GetByUserNameAsync(accountCreate.UserName);
                if (userName != null)
                {
                    ModelState.AddModelError("UserNameOrEmail", "UserNameOrEmail đã tồn tại. Vui lòng chọn một UserNameOrEmail khác.");
                    return ModelInvalid();
                }

                var response = await _accountService.SignUpAsync(accountCreate);

                if (response == null)
                {
                    return Error("Đăng kí KHÔNG thành công");
                }

                return Success(response, "Đăng ký tài khoản thành công! Vui lòng kiểm tra email để lấy mã OTP xác thực tài khoản.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                return Error($"Lỗi đăng kí: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("authen")]
        public async Task<IActionResult> SignInAsync([FromBody] AuthenDTO authenDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ModelInvalid();
                }

                var user = await _identityService.GetByEmailOrUserNameAsync(authenDTO.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("UserNameOrEmail", "Tên đăng nhập hoặc Email không đúng.");
                    return ModelInvalid();
                }

                if (user.IsBlocked)
                {
                    return GetUnAuthorized("Tài khoản của bạn đã bị chặn. Vui lòng liên hệ quản trị viên.");
                }

                // Kiểm tra email confirmed cho Customer
                if (user.Role == DAL.Enums.Role.Customer && !user.EmailConfirmed)
                {
                    return GetUnAuthorized("Vui lòng xác thực email trước khi đăng nhập. Kiểm tra hộp thư để lấy mã OTP.");
                }

                var password = await _identityService.CheckPasswordAsync(user, authenDTO.Password);
                if (!password)
                {
                    ModelState.AddModelError("Password", "Mật khẩu không đúng.");
                    return ModelInvalid();
                }

                var response = await _identityService.PasswordSignInAsync(user, authenDTO.Password, true, true);
                if (response.IsLockedOut)
                {
                    return GetUnAuthorized($"Tài khoản của bạn đã bị khóa vì đăng nhập sai nhiều lần. Khóa đến: {user.LockoutEnd.Value.LocalDateTime}.");
                }
                await _identityService.ResetAccessFailedCountAsync(user);
                var token = await _accountService.GenerateTokenAsync(user);
                if (token == null)
                {
                    return Error("Lỗi sinh mã đăng nhập. Vui lòng thử lại sau ít phút.");
                }

                var authResult = new
                {
                    accessToken = token.AccessToken,
                    refreshToken = token.RefreshToken
                };
                return Success(authResult, "Đăng nhập thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi đăng nhập: {ex.Message}");
            }
        }


        [Authorize]
        [HttpPost]
        [Route("sign-out")]
        public async Task<IActionResult> SignOutAsync([FromBody] SignOutDTO signOutDTO)
        {
            try
            {
                var user = await _identityService.GetByIdAsync(UserId);
                if (user == null)
                {
                    return GetNotFound("Không tìm thấy người dùng.");
                }

                if (!ModelState.IsValid)
                {
                    return ModelInvalid();
                }
                await _identityService.SignOutAsync();
                var response = await _accountService.SignOutAsync(signOutDTO);
                if (!response.IsSuccess)
                {
                    return Error(response.Message);
                }
                return SaveSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi đăng xuất: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("renew-token")]
        public async Task<IActionResult> RenewTokenAsync(AuthenResultDTO authenResult)
        {
            try
            {
                if (string.IsNullOrEmpty(authenResult.AccessToken) || string.IsNullOrEmpty(authenResult.RefreshToken))
                {
                    ModelState.AddModelError("Token", "Token không được để trống.");
                    ModelState.AddModelError("RefreshToken", "RefreshToken không được để trống");
                    return ModelInvalid();
                }

                var checkToken = await _accountService.CheckToRenewTokenAsync(authenResult);
                if (!checkToken.IsSuccess)
                {
                    return Error(checkToken.Message);
                }
                var newToken = await _accountService.GenerateTokenFromRefreshTokenAsync(authenResult);
                if (newToken == null)
                {
                    return Error("Tạo mã đăng nhập mới không thành công. Vui lòng thử lại sau ít phút.");
                }


                var renewResult = new
                {
                    accessToken = newToken.AccessToken,
                    refreshToken = newToken.RefreshToken
                };
                return SaveSuccess(renewResult);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi tạo mã đăng nhập mới: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] AccountForgotPasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                var response = await _accountService.ForgotPasswordAsync(dto);
                if (!response.IsSuccess) return GetError(response.Message);
                return GetSuccess(response);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi tạo mã đổi mật khẩu: {ex.Message}");
            }

        }

        [HttpGet]
        [Route("reset-password-view")]
        public async Task<IActionResult> GetResetPassRequest([FromQuery] string IdToken, [FromQuery] string email)
        {
            try
            {
                return GetSuccess(new { passwordResetToken = IdToken, Email = email });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi nhận yêu cầu đổi mật khẩu: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(AccountResetpassDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();
                var result = await _accountService.ResetPasswordAsync(dto);
                if (!result.IsSuccess) return SaveError(result);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi thay đổi mật khẩu: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(AccountChangePassDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return ModelInvalid();

                var user = await _identityService.GetByIdAsync(UserId);
                if (user != null)
                {
                    var rightPassword = await _identityService.CheckPasswordAsync(user, dto.OldPassword);
                    if (!rightPassword)
                    {
                        ModelState.AddModelError("OldPassword", "Mật khẩu hiện tại không đúng!");
                        return ModelInvalid();
                    }

                    var passwordResetToken = await _identityService.GeneratePasswordResetTokenAsync(user);

                    var success = await _identityService.ResetPasswordAsync(user, passwordResetToken, dto.NewPassword);
                    if (!success.Succeeded) return Error($"Đặt lại mật khẩu thất bại: {GetIdentityErrorMessage(success)}");

                    var newAuthResult = await _accountService.GenerateTokenAsync(user);
                    if (newAuthResult == null) return Error("Đã có lỗi xảy ra trong quá trình sinh mã đăng nhập mới. Xin vui lòng thử lại sau ít phút.");

                    var result = new
                    {
                        accessToken = newAuthResult.AccessToken,
                        refreshToken = newAuthResult.RefreshToken
                    };
                    return SaveSuccess(result);
                }

                return Error("Người dùng không tồn tại.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi thay đổi mật khẩu: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("send-otp")]
        public async Task<IActionResult> SendOTPByEmailAsync(SendOtpDTO email)
        {
            try
            {

                if (string.IsNullOrEmpty(email.Email))
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                    return ModelInvalid();
                }


                var response = await _accountService.SendOTPByEmailAsync(email.Email);
                if (!response.IsSuccess)
                {
                    return Error(response.Message);
                }

                return Success(response, "Gửi OPT thành công");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return Error($"Lỗi gửi mã OTP: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("confirm-email-by-otp")]
        public async Task<IActionResult> ConfirmEmailByOTPAsync([FromBody] VerifyOtpDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ModelInvalid();
                }

                if (string.IsNullOrEmpty(dto.Email))
                {
                    ModelState.AddModelError("Email", "Email không được để trống.");
                    return ModelInvalid();
                }
                if (string.IsNullOrEmpty(dto.Code))
                {
                    ModelState.AddModelError("Code", "Mã OTP không được để trống.");
                    return ModelInvalid();
                }

                var response = await _accountService.ConfirmEmailByOTPAsync(dto.Email, dto.Code);

                if (!response.IsSuccess)
                {
                    return Error(response.Message);
                }

                return Success(null, response.Message);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
                return Error($"Lỗi xác thực email: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("authen-google")]
        public async Task<IActionResult> SignInWithGoogleAsync([FromBody] GoogleAuthDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.IdToken))
                {
                    ModelState.AddModelError("IdToken", "IdToken không được để trống.");
                    return ModelInvalid();
                }

                Console.WriteLine($"Received Google IdToken: {dto.IdToken?.Substring(0, Math.Min(50, dto.IdToken?.Length ?? 0))}...");

                var token = await _accountService.SignInWithGoogleAsync(dto);
                if (token == null)
                {
                    Console.WriteLine("Google authentication failed - check server logs for details");
                    return GetUnAuthorized("Đăng nhập bằng Google không thành công. Vui lòng kiểm tra token và thử lại.");
                }

                Console.WriteLine("Google authentication successful");
                var googleAuthResult = new
                {
                    accessToken = token.AccessToken,
                    refreshToken = token.RefreshToken
                };
                return Success(googleAuthResult, "Đăng nhập thành công.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Controller Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
                return Error($"Lỗi đăng nhập bằng Google: {ex.Message}");
            }
        }

        [HttpPost("staff")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateStaffAccount([FromBody] StaffAccountRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _accountService.CreateStaffAccountAsync(dto);
            if (result == null)
                return GetError("Không thể tạo tài khoản. Email có thể đã tồn tại hoặc Role không hợp lệ.");

            return SaveSuccess(result, "Tạo tài khoản thành công.");
        }

        [HttpPost("staff/import")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ImportStaffAccounts(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return GetError("File không được để trống.");

            if (!file.FileName.EndsWith(".xlsx"))
                return GetError("Chỉ hỗ trợ file Excel (.xlsx).");

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _accountService.ImportStaffAccountsFromExcelAsync(stream);
                return GetSuccess(result);
            }
            catch (Exception ex)
            {
                return GetError($"Không thể import file: {ex.Message}");
            }
        }

        [HttpPut("staff/{userId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateStaffAccount(int userId, [FromBody] StaffAccountUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return ModelInvalid();

            var result = await _accountService.UpdateStaffAccountAsync(userId, dto);
            if (result == null)
                return GetError("Không thể cập nhật tài khoản. Tài khoản không tồn tại hoặc không phải staff.");

            return Success(result, "Cập nhật tài khoản thành công.");
        }

        [HttpPut("{userId}/toggle-block")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ToggleBlockUser(int userId)
        {
            var result = await _accountService.ToggleBlockUserAsync(userId);
            if (!result)
                return GetError("Không tìm thấy tài khoản.");

            return Success(result, "Thay đổi trạng thái tài khoản thành công.");
        }

        [Authorize]
        [HttpPut("update-push-token")]
        public async Task<IActionResult> UpdatePushToken([FromBody] UpdatePushTokenDTO dto)
        {
            int userId = UserId;
            var result = await _accountService.UpdatePushToken(userId, dto);
            return Success(result,"update token success");

        }
    }
}