using Microsoft.AspNetCore.Builder;
using TeamIndia.TalentFlow.API.Middleware;

namespace TeamIndia.TalentFlow.API.Extensions;

public static class MiddlewareExtensions
{
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
