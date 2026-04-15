namespace Coursera_Submission.Middleware;

/// <summary>
/// Logging Middleware — logs every HTTP request and response.
/// Captures method, path, status code, and elapsed time.
/// Copilot was used to help structure the stopwatch pattern and response interception.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // Log incoming request
        _logger.LogInformation(
            "[REQUEST]  {Method} {Path}{QueryString} | ClientIP: {IP}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            context.Connection.RemoteIpAddress);

        // Call the next middleware / endpoint
        await _next(context);

        watch.Stop();

        // Log outgoing response
        var level = context.Response.StatusCode >= 400
            ? LogLevel.Warning
            : LogLevel.Information;

        _logger.Log(level,
            "[RESPONSE] {Method} {Path} => {StatusCode} ({ElapsedMs} ms)",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            watch.ElapsedMilliseconds);
    }
}

// Extension method for clean registration in Program.cs
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLoggingMiddleware>();
}
