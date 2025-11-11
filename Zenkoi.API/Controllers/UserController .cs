using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs;
using Zenkoi.BLL.DTOs.ApplicationUserDTOs;
using Zenkoi.BLL.Services.Interfaces;
using Zenkoi.DAL.Enums;

namespace Zenkoi.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : BaseAPIController
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

	[HttpGet]
	[Route("by-role")]
	public async Task<IActionResult> GetUsersByRole([FromQuery] Role? role = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
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

			var data = await _userService.GetUsersByRoleAsync(role, pageIndex, pageSize, search);
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
	}
}
