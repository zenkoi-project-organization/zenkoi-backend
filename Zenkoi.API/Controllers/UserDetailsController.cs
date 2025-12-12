using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.UserDetailDTOs;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserDetailsController : BaseAPIController
	{
		private readonly IUserDetailService _userDetailService;

		public UserDetailsController(IUserDetailService userDetailService)
		{
			_userDetailService = userDetailService;
		}

		[Authorize]
		[HttpPost]
		[Route("create-update-user-detail")]
		public async Task<IActionResult> CreateUpdateUserDetail(UserDetailRequestDTO dto)
		{
			try
			{
				if (!ModelState.IsValid) return ModelInvalid();

				if (dto.Gender != null)
				{
					if (!dto.Gender.Equals("Male") && !dto.Gender.Equals("Female"))
					{
						ModelState.AddModelError("Gender", "Giới tính chỉ nhận Male hoặc Female.");
						return ModelInvalid();
					}
				}
				var response = await _userDetailService.CreateUpdateUserDetail(dto, UserId);
				if (!response.IsSuccess) return SaveError(response.Message);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user details: {ex.Message}");
			}
		}


		[HttpGet]
		[Route("get-user-detail")]
		public async Task<IActionResult> GetUserDetailByUserId(int userId)
		{
			try
			{
				var response = await _userDetailService.GetUserDetailByUserId(userId);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user details: {ex.Message}");
			}
		}


		[HttpGet]
		[Route("get-all-details/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetAllUserDetails([FromRoute] int pageIndex, [FromRoute] int pageSize)
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
				var data = await _userDetailService.GetAllUserDetails(pageIndex, pageSize);
				var response = new PagingDTO<UserDetailResponseDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user details: {ex.Message}");
			}
		}


		[HttpGet]
		[Route("filter-all-details-by-name/{pageIndex}/{pageSize}")]
		public async Task<IActionResult> GetAllUserDetailsByName([FromRoute] int pageIndex, [FromRoute] int pageSize, [FromQuery] string? name)
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
				var data = await _userDetailService.GetAllUserDetailsByName(pageIndex, pageSize, name);
				var response = new PagingDTO<UserDetailResponseDTO>(data);
				if (response == null) return GetError();
				return GetSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user details: {ex.Message}");
			}
		}
		[Authorize]
        [HttpGet("get-me")]
        public async Task<IActionResult> GetMe()
        {
            if (UserId < 0)
                return Unauthorized("Không tìm thấy thông tin người dùng trong token.");

            var response = await _userDetailService.GetUserDetailByUserId(UserId);
            return response == null ? GetError("Không tìm thấy thông tin người dùng.") : GetSuccess(response);
        }

		[Authorize]
		[HttpPut("update-avatar")]
		public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDTO dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return ModelInvalid();

				var response = await _userDetailService.UpdateAvatarAsync(dto, UserId);
				if (!response.IsSuccess)
					return SaveError(response.Message);

				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi cập nhật avatar: {ex.Message}");
			}
		}

        [HttpDelete]
		[Route("delete-user-detail")]
		public async Task<IActionResult> DeleteUserDetail()
		{
			try
			{
				var response = await _userDetailService.DeleteUserDetail(UserId);
				if (!response.IsSuccess) return SaveError(response.Message);
				return SaveSuccess(response);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
				return Error($"Lỗi xử lý user details: {ex.Message}");
			}
		}
	}
}
