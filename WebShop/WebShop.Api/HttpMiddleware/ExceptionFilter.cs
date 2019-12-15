using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebShop.Api.HttpMiddleware
{
    public class ExceptionFilter
    {
        private readonly RequestDelegate _next;

        public ExceptionFilter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                await HandleEx(httpContext, e);
            }
        }

        private async Task HandleEx(HttpContext httpContext, Exception ex)
        {
            if (ex is UnauthorizedAccessException unauthorizedAccessException)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            
            httpContext.Response.StatusCode = httpContext.Response.StatusCode == default 
                ? StatusCodes.Status400BadRequest
                : httpContext.Response.StatusCode;
            
            await JsonSerializer.SerializeAsync(httpContext.Response.Body, new {message = ex.Message});
            await _next(httpContext);
        }
    }
}