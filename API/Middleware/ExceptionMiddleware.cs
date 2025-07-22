using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{message}", ex.Message);
            context.Response.ContentType = "application/json";

            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = ex.Message;
            string details = env.IsDevelopment() ? ex.StackTrace?.ToString() : "Internal Server Error";

            if (ex is BadRequestException)
                statusCode = (int)HttpStatusCode.BadRequest;
            else if (ex is UnauthorizedException)
                statusCode = (int)HttpStatusCode.Unauthorized;
            else if (ex is NotFoundException)
                statusCode = (int)HttpStatusCode.NotFound;

            context.Response.StatusCode = statusCode;

            var response = new ApiException(statusCode, message, details);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            
            var json = JsonSerializer.Serialize(response, options);
            
            await context.Response.WriteAsync(json);
        }
    }
}