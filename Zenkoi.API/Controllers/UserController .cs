using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.BLL.Services.Implements;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Enums;

namespace Zenkoi.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : BaseAPIController
	{
		private readonly IUserService _userService;
		private readonly ExpoPushNotificationService _pushService;

		public UsersController(IUserService userService, ExpoPushNotificationService expoPushNotificationService)
		{
			_userService = userService;
            _pushService = expoPushNotificationService;

        }


        [HttpPost("send-notify")]
        public async Task<IActionResult> SendTestNotify()
        {        
            string mockToken = "ExponentPushToken[xxxxxxxxxxxxxxxxxxxx]";

            await _pushService.SendAsync(
                mockToken,
                "Test Notification",
                "Đây là thử nghiệm Backend Push Notification"
            );

            return Ok("Request đã gửi tới Expo server. Xem console log để thấy response.");
        }

        [HttpGet]
		[Route("by-role")]
		public async Task<IActionResult> GetUsersByRole([FromQuery] Role? role = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isBlocked = null)
		{
			try
			{
				if (pageIndex <= 0)
				{
					return GetError("Page Index phải là số nguyên dương.");
				}

				if (pageSize <= 0)
				{
					return GetError("Page Size phải là số nguyên dương.");
				}

				var data = await _userService.GetUsersByRoleAsync(role, pageIndex, pageSize, search, isBlocked);
				var response = new PagingDTO<ApplicationUserResponseDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user: {ex.Message}");
			}
		}

		/// <summary>
		/// Update user profile (fullname, phone, userdetail)
		/// </summary>
		[Authorize]
		[HttpPut]
		[Route("profile")]
		public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileDTO dto)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return ModelInvalid();
				}

				var result = await _userService.UpdateUserProfileAsync(UserId, dto);
				return SaveSuccess(result, "Cập nhật thông tin người dùng thành công.");
			}
			catch (ArgumentException ex)
			{
				return GetError(ex.Message);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi cập nhật thông tin người dùng: {ex.Message}");
			}
		}
	}
}
