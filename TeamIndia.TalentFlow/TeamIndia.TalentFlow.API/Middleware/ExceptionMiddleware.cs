using System.Text.Json;
using TeamIndia.TalentFlow.Application.Common;

namespace TeamIndia.TalentFlow.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred while processing request.");

            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "An unexpected error occurred.";
            IEnumerable<string>? errors = null;

            if (ex is TeamIndia.TalentFlow.API.Exceptions.ApiException apiEx)
            {
                statusCode = apiEx.StatusCode;
                message = apiEx.Message;
                errors = apiEx.Errors;
            }
            else if (ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                statusCode = StatusCodes.Status409Conflict;
                message = "A concurrency error occurred.";
            }

            context.Response.StatusCode = statusCode;

            var env = context.RequestServices.GetService(typeof(Microsoft.Extensions.Hosting.IHostEnvironment)) as Microsoft.Extensions.Hosting.IHostEnvironment;
            var responseMessage = message;
            if (env != null && env.IsDevelopment() && !(ex is TeamIndia.TalentFlow.API.Exceptions.ApiException))
            {
                responseMessage = ex.Message;
            }

            var response = BaseResponse.Fail(responseMessage, errors, statusCode);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
