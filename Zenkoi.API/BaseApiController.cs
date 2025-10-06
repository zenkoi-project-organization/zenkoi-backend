using Microsoft.AspNetCore.Mvc;
using Zenkoi.BLL.DTOs.Response;

namespace Zenkoi.API
{
	public class BaseAPIController : ControllerBase
	{
		protected ActionResult Error(string message, object data = null)
		{
			return new BadRequestObjectResult(new ResponseApiDTO
			{
				Result = data,
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				Message = message,
			});
		}

		protected ActionResult GetNotFound(string message)
		{
			return new NotFoundObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = System.Net.HttpStatusCode.NotFound
			});
		}

		protected ActionResult GetUnAuthorized(string message)
		{
			return new UnauthorizedObjectResult(new ResponseApiDTO
			{
				IsSuccess = false,
				Message = message,
				StatusCode = System.Net.HttpStatusCode.Unauthorized
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
				.ToDictionary(
					kvp => kvp.Key,
					kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).First()).ToList();
			return new BadRequestObjectResult(new ResponseApiDTO
			{
				Errors = errors,
				StatusCode = System.Net.HttpStatusCode.BadRequest,
				Message = "Save data failed"
			});
		}

		protected ActionResult Success(object data, string message)
		{
			return new OkObjectResult(new ResponseApiDTO
			{
				Result = data,
				StatusCode = System.Net.HttpStatusCode.OK,
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
