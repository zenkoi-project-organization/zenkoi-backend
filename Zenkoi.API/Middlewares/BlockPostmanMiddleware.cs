namespace Zenkoi.API.Middlewares
{
	public class BlockPostmanMiddleware
	{
		private readonly RequestDelegate _next;

		public BlockPostmanMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var userAgent = context.Request.Headers["User-Agent"].ToString();

			if (userAgent.Contains("PostmanRuntime", StringComparison.OrdinalIgnoreCase))
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsync("Request blocked: Postman is not allowed.");
				return;
			}

			await _next(context);
		}
	}

}
