using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Net.Http.Json;
using System.Net.Http;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Zenkoi.BLL.DTOs.AccountDTOs;
using Zenkoi.DAL.Entities;
using Zenkoi.DAL.Queries;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.DTOs.EmailDTOs;
using Zenkoi.DAL.Enums;

namespace Zenkoi.BLL.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;     
		private readonly ICustomerService _customerService;

		private readonly int PLAN_FREE_ID = 1;

        public AccountService(IIdentityService identityService, IUnitOfWork unitOfWork,
                               IEmailService emailService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _emailService = emailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;   
			_customerService = customerService;
        }
        public async Task<AuthenResultDTO> GenerateTokenAsync(ApplicationUser user)
        {
            try
            {
                var authClaims = new List<Claim>
            {
                new Claim("Email", user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim("Role", user.Role.ToString()),
                new Claim("Name", user.UserName)
            };

                var userRoles = await _identityService.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var jwtToken = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.UtcNow.AddDays(1),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512)
                    );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                var refreshToken = GenerateRefreshToken();
                var refreshTokenInDb = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    JwtId = jwtToken.Id,
                    UserId = user.Id,
                    Token = refreshToken,
                    IsUsed = false,
                    IsRevoked = false,
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.Now.AddDays(30),
                };

                await _unitOfWork.BeginTransactionAsync();

                var refreshTokenRepo = _unitOfWork.GetRepo<RefreshToken>();
                var refreshTokenByIds = await refreshTokenRepo.Get(new QueryBuilder<RefreshToken>()
                                                            .WithPredicate(x => x.UserId.Equals(user.Id))
                                                            .WithTracking(true)
                                                            .Build()).ToListAsync();
                foreach (var item in refreshTokenByIds)
                {
                    await refreshTokenRepo.DeleteAsync(item);
                }
                await refreshTokenRepo.CreateAsync(refreshTokenInDb);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new AuthenResultDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                return null;
                throw;
            }
        }

        public async Task<BaseResponse> CheckToRenewTokenAsync(AuthenResultDTO authenResult)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false
            };

            try
            {
                var tokenInVerification = jwtTokenHandler.ValidateToken(authenResult.AccessToken, tokenValidateParam, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    Console.WriteLine("Algorithm: " + jwtSecurityToken.Header.Alg);
                    Console.WriteLine(SecurityAlgorithms.HmacSha512);
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return new BaseResponse
                        {
                            IsSuccess = false,
                            Message = "Access token không hợp lệ"
                        };
                    }
                }

                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Access token chưa hết hạn."
                    };
                }
                var refreshTokenRepo = _unitOfWork.GetRepo<RefreshToken>();
                var storedToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
                                                                        .WithPredicate(x => x.Token.Equals(authenResult.RefreshToken))
                                                                        .WithTracking(false)
                                                                        .Build());
                if (storedToken == null)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh Token không tồn tại."
                    };
                }

                if (storedToken.IsUsed)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token đã được sử dụng."
                    };
                }
                if (storedToken.IsRevoked)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token đã bị thu hồi."
                    };
                }

                if (storedToken.ExpiredAt < DateTime.Now)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Refresh token đã hết hạn."
                    };
                }

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Token không khớp."
                    };
                }
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Token hợp lệ."
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AuthenResultDTO> GenerateTokenFromRefreshTokenAsync(AuthenResultDTO authenResult)
        {
            try
            {
                var refreshTokenRepo = _unitOfWork.GetRepo<RefreshToken>();
                var storedToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
                    .WithPredicate(x => x.Token == authenResult.RefreshToken && !x.IsUsed && !x.IsRevoked)
                    .WithTracking(true)
                    .Build());

                if (storedToken == null)
                {
                    return null;
                }

                // Đánh dấu là đã sử dụng
                storedToken.IsUsed = true;
                await _unitOfWork.SaveChangesAsync();

                // Lấy user từ UserId
                var user = await _identityService.GetByIdAsync(storedToken.UserId);
                if (user == null)
                {
                    return null;
                }

                // Sinh token mới như cũ
                return await GenerateTokenAsync(user);
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackAsync();
                return null;
            }
        }





		public async Task<BaseResponse> SignOutAsync(SignOutDTO signOutDTO)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var refreshTokenRepo = _unitOfWork.GetRepo<RefreshToken>();
				var refreshToken = await refreshTokenRepo.GetSingleAsync(new QueryBuilder<RefreshToken>()
																			.WithPredicate(x => x.Token.Equals(signOutDTO.RefreshToken))
																			.WithTracking(true)
																			.WithInclude(x => x.ApplicationUser)
																			.Build());

				if (refreshToken == null)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Refresh Token không hợp lệ."
					};
				}

				if (refreshToken.IsUsed || refreshToken.IsRevoked)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Refresh token đã được sử dụng hoặc thu hồi."
					};
				}

				refreshToken.IsRevoked = true;
				await refreshTokenRepo.UpdateAsync(refreshToken);
				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();
				return new BaseResponse
				{
					IsSuccess = true,
					Message = "Đăng xuất thành công."
				};
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<AccountViewDTO> SignUpAsync(AccountCreateRequestDTO accRequest)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();
				var user = new ApplicationUser
				{
					Role = Role.Customer,
					Email = accRequest.Email,
					UserName = accRequest.UserName,
					PhoneNumber = accRequest.PhoneNumber,
					FullName = accRequest.FullName,
					EmailConfirmed = false
				};

				var createResult = await _identityService.CreateAsync(user, accRequest.Password);
				if (!createResult.Succeeded)
				{
					var errors = createResult.Errors.Select(e => e.Description).ToList();
					var errorMessage = string.Join("; ", errors);
					throw new Exception(errorMessage);
				}

				await _identityService.AddToRoleAsync(user, Role.Customer.ToString());
                await _customerService.CreateCustomerProfileAsync(user.Id);

				var userDetailRepo = _unitOfWork.GetRepo<UserDetail>();
				var userDetail = new UserDetail
				{
					ApplicationUserId = user.Id,
					Gender = Gender.Male
				};
				await userDetailRepo.CreateAsync(userDetail);

                await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				await SendOTPByEmailAsync(user.Email);

				return new AccountViewDTO
				{
					Id = user.Id.ToString(),
					EmailAddress = user.Email,
					UserName = user.UserName,
					PhoneNumber = user.PhoneNumber,

				};
			}
			catch (Exception)
			{
				await _unitOfWork.RollBackAsync();
				throw;
			}
		}

		public async Task<BaseResponse> ForgotPasswordAsync(AccountForgotPasswordDTO dto)
		{
			var user = await _identityService.GetByEmailAsync(dto.Email);

			if (user == null)
			{
				return new BaseResponse
				{
					IsSuccess = true,
					Message = "Nếu email tồn tại thì link xác thực đã được gửi đến. Hãy đảm bảo email của bạn chính xác và truy cập vào link xác thực để thay đổi mật khẩu nhé."
				};
			}

			var passwordResetToken = await _identityService.GeneratePasswordResetTokenAsync(user);
			var encodedPasswordToken = HttpUtility.UrlEncode(passwordResetToken);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"encode password reset token: {encodedPasswordToken}");
			Console.ResetColor();
			var forgotUrl = $"{_configuration["FronendURL"]}/renew-password?token={encodedPasswordToken}&email={user.Email}";
            var emailContent = $@"
				<div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: auto; border: 1px solid #dfe6e9; border-radius: 12px; padding: 28px; background: #ffffff; box-shadow: 0 4px 16px rgba(0,0,0,0.05);"">
				  <h2 style=""color: #e67e22; text-align: center; margin-bottom: 20px;"">Yêu cầu đặt lại mật khẩu</h2>				 
				  <p style=""color: #2d3436;"">Xin chào <b>{user.FullName ?? user.Email}</b>,</p>
				  <p style=""color: #2d3436;"">Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản ZenKoi của bạn.</p>
				  <p style=""color: #2d3436;"">Hãy nhấn vào nút bên dưới để tạo mật khẩu mới:</p>

				  <div style=""text-align: center; margin: 28px 0;"">
					<a href=""{forgotUrl}"" style=""display: inline-block; padding: 14px 32px; background: linear-gradient(135deg, #3498db, #2980b9); color: #fff; border-radius: 6px; text-decoration: none; font-weight: 600; font-size: 16px; transition: opacity 0.3s ease;"">Đặt lại mật khẩu</a>
				  </div>

				  <p style=""color: #95a5a6; font-size: 14px; text-align: center;"">Nếu bạn không gửi yêu cầu này, vui lòng bỏ qua email.</p>

				  <hr style=""border: none; border-top: 1px solid #ecf0f1; margin: 28px 0;""/>

				  <p style=""font-size: 12px; color: #b2bec3; text-align: center;"">🐟 Cảm ơn bạn đã tin tưởng ZenKoi<br/>&copy; Đội ngũ ZenKoi</p>
				</div>";
            var message = new EmailDTO(
				new string[] { user.Email! },
				"Yêu cầu đổi mật khẩu",
				emailContent
			);
            
            Console.WriteLine("Sending forgot password email...");
            var emailResult = _emailService.SendEmail(message);
            
            if (!emailResult.IsSuccess)
            {
                Console.WriteLine($"Forgot password email failed: {emailResult.Message}");
                return new BaseResponse 
                { 
                    IsSuccess = false, 
                    Message = $"Không thể gửi email: {emailResult.Message}" 
                };
            }
            
            Console.WriteLine("Forgot password email sent successfully");
			return new BaseResponse { IsSuccess = true, Message = "Url đổi mật khẩu đã được gửi đến email của bạn. Hãy truy cập url để đổi mật khẩu nhé." };
		}

		public async Task<BaseResponse> ResetPasswordAsync(AccountResetpassDTO dto)
		{
			try
			{
				var user = await _identityService.GetByEmailAsync(dto.Email);
				if (user == null)
				{
					return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy người dùng." };
				}

				var decodedPasswordToken = HttpUtility.UrlDecode(dto.AccsessToken);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"decoded password reset token: {decodedPasswordToken}");
				Console.ResetColor();
				if (decodedPasswordToken.Contains(' '))
				{
					decodedPasswordToken = decodedPasswordToken.Replace(" ", "+");
				}
				var result = await _identityService.ResetPasswordAsync(user, decodedPasswordToken, dto.NewPassword);
				if (!result.Succeeded)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Thay đổi mật khẩu không thành công. Hãy kiểm tra lại Email hoặc Mật Khẩu của bạn."
					};
				}
				return new BaseResponse
				{
					IsSuccess = true,
					Message = "Mật khẩu đã thay đổi thành công."
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<BaseResponse> SendOTPByEmailAsync(string email)
		{
			try
			{
				Console.WriteLine($"Starting OTP generation for email: {email}");
				var user = await _identityService.GetByEmailAsync(email);
				if (user == null)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Email không tồn tại trong hệ thống."
					};
				}

				Console.WriteLine("Generating OTP token...");
				var otpToken = await _identityService.GenerateTwoFactorTokenAsync(user, "Email");
				Console.WriteLine($"OTP token generated: {otpToken}");

				var emailContent = $@"
				<div style=""font-family: 'Segoe UI', Arial, sans-serif; max-width: 520px; margin: auto; border: 1px solid #d1e7f5; border-radius: 12px; padding: 28px; background: #ffffff; box-shadow: 0 4px 16px rgba(0,0,0,0.05);"">
				  <h2 style=""color: #3498db; text-align: center; margin-bottom: 20px;"">🐟 Xác thực Email - ZenKoi</h2>
				  <p style=""color: #2d3436;"">Xin chào <b>{user.FullName ?? user.Email}</b>,</p>
				  <p style=""color: #2d3436;"">Đây là mã OTP để xác thực email của bạn:</p>

				  <div style=""text-align: center; margin: 28px 0; padding: 20px; background: linear-gradient(135deg, #e3f2fd, #bbdefb); border-radius: 8px; border: 2px dashed #3498db;"">
					<div style=""font-size: 32px; font-weight: 700; color: #2980b9; letter-spacing: 8px; font-family: 'Courier New', monospace;"">{otpToken}</div>
				  </div>

				  <p style=""color: #e74c3c; font-size: 14px; text-align: center; font-weight: 600;"">⚠️ Mã OTP có hiệu lực trong 5 phút</p>
				  <p style=""color: #95a5a6; font-size: 14px; text-align: center;"">Vui lòng <b>KHÔNG chia sẻ</b> mã này với bất kỳ ai.</p>
				  <p style=""color: #95a5a6; font-size: 14px; text-align: center;"">Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.</p>

				  <hr style=""border: none; border-top: 1px solid #ecf0f1; margin: 28px 0;""/>

				  <p style=""font-size: 12px; color: #b2bec3; text-align: center;"">🐟 Cảm ơn bạn đã tin tưởng ZenKoi<br/>&copy; Đội ngũ ZenKoi</p>
				</div>";

				var emailDTO = new EmailDTO
				(
					new string[] { email },
					"Mã OTP xác thực - ZenKoi",
					emailContent
				);

				Console.WriteLine("Sending OTP email...");
				var emailResult = _emailService.SendEmail(emailDTO);
				
				if (!emailResult.IsSuccess)
				{
					Console.WriteLine($"Email sending failed: {emailResult.Message}");
					return new BaseResponse
					{
						IsSuccess = false,
						Message = $"Không thể gửi email: {emailResult.Message}"
					};
				}

				Console.WriteLine("OTP email sent successfully");
				return new BaseResponse
				{
					IsSuccess = true,
					Message = $"Mã OTP đã được gửi đến email {email}."
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception in SendOTPByEmailAsync: {ex.Message}");
				Console.WriteLine($"Stack Trace: {ex.StackTrace}");
				
				return new BaseResponse
				{
					IsSuccess = false,
					Message = $"Lỗi gửi mã OTP: {ex.Message}"
				};
			}
		}

		public async Task<BaseResponse> ConfirmEmailByOTPAsync(string email, string otp)
		{
			try
			{
				var user = await _identityService.GetByEmailAsync(email);
				if (user == null)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Không tìm thấy tài khoản với email này."
					};
				}

				if (user.EmailConfirmed)
				{
					return new BaseResponse
					{
						IsSuccess = true,
						Message = "Email đã được xác thực trước đó."
					};
				}

				// Verify OTP
				var isValid = await _identityService.VerifyTwoFactorTokenAsync(user, "Email", otp);
				if (!isValid)
				{
					return new BaseResponse
					{
						IsSuccess = false,
						Message = "Mã OTP không hợp lệ hoặc đã hết hạn."
					};
				}

				// Confirm email
				user.EmailConfirmed = true;
				var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
				await userRepo.UpdateAsync(user);
				await _unitOfWork.SaveChangesAsync();

				return new BaseResponse
				{
					IsSuccess = true,
					Message = "Xác thực email thành công! Bạn có thể đăng nhập ngay bây giờ."
				};
			}
			catch (Exception ex)
			{
				return new BaseResponse
				{
					IsSuccess = false,
					Message = $"Lỗi xác thực email: {ex.Message}"
				};
			}
		}
		
		public async Task<AuthenResultDTO> SignInWithGoogleAsync(GoogleAuthDTO dto)
		{
			try
			{
				var clientId = _configuration["Authentication:Google:ClientId"];
				Console.WriteLine($"Google ClientId: {clientId}");
				
				if (string.IsNullOrEmpty(clientId))
				{
					Console.WriteLine("ERROR: Google ClientId is not configured");
					return null;
				}

				var httpClient = new HttpClient();
				var tokenInfoUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={dto.IdToken}";
				Console.WriteLine($"Calling Google API: {tokenInfoUrl}");
				
				var response = await httpClient.GetAsync(tokenInfoUrl);
				Console.WriteLine($"Google API Response Status: {response.StatusCode}");
				
				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"Google API Error: {errorContent}");
					return null;
				}

				var responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Google API Response: {responseContent}");

				var tokenInfo = await response.Content.ReadFromJsonAsync<GoogleTokenInfo>();
				if (tokenInfo == null)
				{
					Console.WriteLine("ERROR: Failed to parse Google token info");
					return null;
				}

				Console.WriteLine($"Token Info - Aud: {tokenInfo.Aud}, Email: {tokenInfo.Email}, Name: {tokenInfo.Name}");
				
				if (tokenInfo.Aud != clientId)
				{
					Console.WriteLine($"ERROR: Audience mismatch. Expected: {clientId}, Got: {tokenInfo.Aud}");
					return null;
				}

				// Get or create user
				Console.WriteLine($"Looking for user with email: {tokenInfo.Email}");
				var user = await _identityService.GetByEmailAsync(tokenInfo.Email);
				
				if (user != null && user.IsBlocked)
				{
					Console.WriteLine("ERROR: User is blocked");
					return null;
				}
		
				if (user == null)
				{
					Console.WriteLine("Creating new user...");
					user = new ApplicationUser
					{
						Email = tokenInfo.Email,
						UserName = tokenInfo.Email,
						FullName = tokenInfo.Name,
						EmailConfirmed = true,
						Role = DAL.Enums.Role.Customer
					};

					var createResult = await _identityService.CreateAsync(user, Guid.NewGuid().ToString() + "Aa1!");
					if (!createResult.Succeeded)
					{
						Console.WriteLine($"ERROR: Failed to create user. Errors: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
						return null;
					}
					
					var roleResult = await _identityService.AddToRoleAsync(user, user.Role.ToString());
					if (!roleResult.Succeeded)
					{
						Console.WriteLine($"ERROR: Failed to add role to user. Errors: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
						return null;
					}

					if (user.Role == DAL.Enums.Role.Customer)
					{
						await _customerService.CreateCustomerProfileAsync(user.Id);
					}

					Console.WriteLine("User created successfully");
				}
				else
				{
					Console.WriteLine("User found in database");
				}

				// Store avatar
				Console.WriteLine("Processing user details...");
				var userDetailRepo = _unitOfWork.GetRepo<UserDetail>();
				var userDetail = await userDetailRepo.GetSingleAsync(new QueryBuilder<UserDetail>()
				    .WithPredicate(x => x.ApplicationUserId == user.Id)
				    .WithTracking(true)
				    .Build());

                if (userDetail == null)
                {
                    Console.WriteLine("Creating user detail with avatar...");
                    userDetail = new UserDetail
                    {
                        ApplicationUserId = user.Id,
                        AvatarURL = tokenInfo.Picture
                    };
                    await userDetailRepo.CreateAsync(userDetail);
                }
                else
                {
                    Console.WriteLine("User detail already exists");
                }
               
                await _unitOfWork.SaveChangesAsync();
                Console.WriteLine("User details saved successfully");

                Console.WriteLine("Generating authentication token...");
                var authResult = await GenerateTokenAsync(user);
                if (authResult == null)
                {
                    Console.WriteLine("ERROR: Failed to generate authentication token");
                    return null;
                }
                
                Console.WriteLine("Google authentication successful");
                return authResult;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"EXCEPTION in SignInWithGoogleAsync: {ex.Message}");
				Console.WriteLine($"Stack Trace: {ex.StackTrace}");
				return null;
			}
		}

		private class GoogleTokenInfo
		{
			public string Iss { get; set; }
			public string Azp { get; set; }
			public string Aud { get; set; }
			public string Sub { get; set; }
			public string Email { get; set; }
			public string Email_Verified { get; set; }
			public string Name { get; set; }
			public string Picture { get; set; }
			public string Given_Name { get; set; }
			public string Family_Name { get; set; }
			public string Iat { get; set; }
			public string Exp { get; set; }
		}

        public async Task<StaffAccountResponseDTO?> CreateStaffAccountAsync(StaffAccountRequestDTO dto)
        {
            if (dto.Role != Role.FarmStaff && dto.Role != Role.SaleStaff)
                return null;

            var existingUser = await _identityService.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return null;

            var tempPassword = string.IsNullOrEmpty(dto.Password) ? GenerateRandomPassword() : dto.Password;

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.UserName,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Role = dto.Role,
                EmailConfirmed = true,
                IsDeleted = false,
                IsBlocked = false
            };

            var result = await _identityService.CreateAsync(user, tempPassword);
            if (!result.Succeeded)
                return null;

            await _identityService.AddToRoleAsync(user, dto.Role.ToString());

            var userDetailRepo = _unitOfWork.GetRepo<UserDetail>();
            var userDetail = new UserDetail
            {
                ApplicationUserId = user.Id,
                Gender = Gender.Male
            };
            await userDetailRepo.CreateAsync(userDetail);
            await _unitOfWork.SaveChangesAsync();

            return new StaffAccountResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                TempPassword = tempPassword
            };
        }

        public async Task<ImportStaffResultDTO> ImportStaffAccountsFromExcelAsync(Stream fileStream)
        {
            var result = new ImportStaffResultDTO();

            try
            {
                using var package = new OfficeOpenXml.ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount <= 1)
                {
                    result.TotalRows = 0;
                    return result;
                }

                result.TotalRows = rowCount - 1;

                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var email = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                        var userName = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                        var fullName = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                        var phoneNumber = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                        var roleStr = worksheet.Cells[row, 5].Value?.ToString()?.Trim();
                        var password = worksheet.Cells[row, 6].Value?.ToString()?.Trim();

                        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName) ||
                            string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(roleStr))
                        {
                            result.FailureCount++;
                            result.Errors.Add(new ImportErrorDTO
                            {
                                RowNumber = row,
                                Email = email ?? "",
                                ErrorMessage = "Thiếu thông tin bắt buộc"
                            });
                            continue;
                        }

                        if (!Enum.TryParse<Role>(roleStr, true, out var role) ||
                            (role != Role.FarmStaff && role != Role.SaleStaff))
                        {
                            result.FailureCount++;
                            result.Errors.Add(new ImportErrorDTO
                            {
                                RowNumber = row,
                                Email = email,
                                ErrorMessage = "Role không hợp lệ (chỉ FarmStaff hoặc SaleStaff)"
                            });
                            continue;
                        }

                        var dto = new StaffAccountRequestDTO
                        {
                            Email = email,
                            UserName = userName,
                            FullName = fullName,
                            PhoneNumber = phoneNumber ?? "",
                            Role = role,
                            Password = password
                        };

                        var createdAccount = await CreateStaffAccountAsync(dto);
                        if (createdAccount != null)
                        {
                            result.SuccessCount++;
                            result.SuccessfulAccounts.Add(createdAccount);
                        }
                        else
                        {
                            result.FailureCount++;
                            result.Errors.Add(new ImportErrorDTO
                            {
                                RowNumber = row,
                                Email = email,
                                ErrorMessage = "Email đã tồn tại hoặc không thể tạo tài khoản"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add(new ImportErrorDTO
                        {
                            RowNumber = row,
                            Email = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            ErrorMessage = $"Lỗi: {ex.Message}"
                        });
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<StaffAccountResponseDTO?> UpdateStaffAccountAsync(int userId, StaffAccountUpdateDTO dto)
        {
            var user = await _identityService.GetByIdAsync(userId);
            if (user == null)
                return null;

            // Manager không được phép update tài khoản Manager khác
            if (user.Role == Role.Manager)
                return null;

            if (user.Role != Role.FarmStaff && user.Role != Role.SaleStaff)
                return null;

            user.UserName = dto.UserName;
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;

            var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new StaffAccountResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                TempPassword = "***"
            };
        }

        public async Task<bool> ToggleBlockUserAsync(int userId)
        {
            var user = await _identityService.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Manager không được phép block/unblock tài khoản Manager khác
            if (user.Role == Role.Manager)
                return false;

            user.IsBlocked = !user.IsBlocked;

            var userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

		#region Private
		private string GenerateRefreshToken()
		{
			var random = new byte[32];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(random);

				return Convert.ToBase64String(random);
			}
		}

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789@#$";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate);
        }

        public async Task<bool> UpdatePushToken(int userId ,UpdatePushTokenDTO dto)
        {
            var _userRepo = _unitOfWork.GetRepo<ApplicationUser>();
            var user = await _identityService.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("không tìm thấy tài khoản");
            }
            var allowedRoles = new[] { Role.FarmStaff, Role.Manager };

            if (!allowedRoles.Contains(user.Role))
            {
                throw new InvalidOperationException("tài khoản không phải là nhân viên của trang trại");
            }
            user.ExpoPushToken = dto.ExpoPushToken;
            await _userRepo.UpdateAsync(user);
            return await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<string>> GetStaffManagerTokensAsync()
        {
            var _userRepo =  _unitOfWork.GetRepo<ApplicationUser>();
            var users = await _userRepo.GetAllAsync(new QueryOptions<ApplicationUser>
            {
                Predicate = u =>
                    u.ExpoPushToken != null &&
                    (u.Role == Role.FarmStaff || u.Role == Role.Manager) &&
                    u.IsDeleted == false &&
                    u.IsBlocked == false,
                Tracked = false
            });

            return users.Select(u => u.ExpoPushToken).ToList();
        }
    

        #endregion

    }
}
