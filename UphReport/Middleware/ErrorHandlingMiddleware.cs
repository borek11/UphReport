using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UphReport.Exceptions;

namespace UphReport.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
		private readonly ILogger<ErrorHandlingMiddleware> _logger;
		//private readonly RequestDelegate _next;

		public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
		{
			_logger = logger;
		}

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
            catch (BadRequestException badRequest)
            {
                context.Response.StatusCode = 400;
				
                await context.Response.WriteAsJsonAsync(badRequest.Message);
            }
            catch (NotFoundException notFound)
            {
                context.Response.StatusCode = 404;

                await context.Response.WriteAsJsonAsync(notFound.Message);
            }
            catch (Exception e)
			{
				_logger.LogError(e, e.Message);

				context.Response.StatusCode = 500;
				await context.Response.WriteAsync("Something went wrong!");
			}
		}
    }
}
