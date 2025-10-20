using System.Net;
using System.Text.Json;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.Helpers.Validations;

namespace Cafe_Web_App.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomValidationException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message, ex.Errors);
            }
            catch (KeyNotFoundException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống", ex.Message);
            }

        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message, object? detail = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ResponseApiDTO
            {
                StatusCode = (int)statusCode,
                Message = message,
                Result = detail,
                IsSuccess = false
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}