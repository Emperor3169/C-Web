using Serilog;

namespace Novskiy.UI.Middleware;

public class FileLogger
{
    private readonly RequestDelegate _next;

    public FileLogger(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        await _next(httpContext);
        var code = httpContext.Response.StatusCode;
        var temp = code / 100;
        
        // Логируем все запросы, которые вернули НЕ 2xx (например, 302, 404, 500)
        if (temp != 2)
        {
            Log.Logger.Information($"---> Request {httpContext.Request.Path} returns {code}");
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class FileLoggerExtensions
{
    public static IApplicationBuilder UseFileLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FileLogger>();
    }
}
