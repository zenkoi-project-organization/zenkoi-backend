using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.Response;
using System.Linq;

namespace Zenkoi.API
{
	public class BaseAPIController : ControllerBase
	{
	protected ActionResult Error(string message, object data = null)
	{
		return new BadRequestObjectResult(new ResponseApiDTO
		{
			Result = null,
			StatusCode = 400,
			Message = message,
			IsSuccess = false
		});
	}

	protected string GetIdentityErrorMessage(Microsoft.AspNetCore.Identity.IdentityResult result)
	{
		if (result == null || !result.Errors.Any())
			return "Lỗi không xác định";

		var errors = result.Errors.Select(e => e.Description).ToList();
		return string.Join("; ", errors);
	}

		protected ActionResult GetNotFound(string message)
		{
			return new NotFoundObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = 404,
				Result = null
			});
		}

		protected ActionResult GetUnAuthorized(string message)
		{
			return new UnauthorizedObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = 401,
				Result = null
			});
		}

		protected ActionResult GetError()
		{
			return Error("Get Data Failed");
		}

		protected ActionResult GetError(string message)
		{
			return Error(message);
		}

		protected ActionResult SaveError(object data = null)
		{
			return Error("Save data failed", data);
		}

	protected ActionResult ModelInvalid()
	{
		var errors = ModelState.Where(m => m.Value.Errors.Count > 0)
			.Select(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).First())
			.ToList();
		
		var errorMessage = errors.Any() ? string.Join("; ", errors) : "Dữ liệu không hợp lệ";
		
		return new BadRequestObjectResult(new ResponseApiDTO
		{
			Result = null,
			StatusCode = 400,
			Message = errorMessage,
			IsSuccess = false
		});
	}

		protected ActionResult Success(object data, string message)
		{
			return new OkObjectResult(new ResponseApiDTO
			{
				Result = data,
				StatusCode = 200,
				Message = message,
				IsSuccess = true
			});
		}

		protected ActionResult GetSuccess(object data)
		{
			return Success(data, "Get data success");
		}

		protected ActionResult SaveSuccess(object data)
		{
			return Success(data, "Save data success");
		}

		protected string UserName => User.FindFirst("Name")?.Value;

		protected string UserEmail => User.FindFirst("Email")?.Value;

		protected int UserId
		{
			get
			{
				try
				{
					var id = int.Parse(User.FindFirst("Id")?.Value);
					return id;
				}
				catch (Exception)
				{
					throw;
				}

			}
		}
	}
}
