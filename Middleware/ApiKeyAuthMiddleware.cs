namespace Coursera_Submission.Middleware;

/// <summary>
/// API Key Authentication Middleware.
/// Protects all /api/* routes by requiring a valid X-Api-Key header.
/// The key is read from configuration (appsettings.json) — never hardcoded.
/// Copilot was used to suggest the configuration-driven key lookup pattern.
/// </summary>
public class ApiKeyAuthMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;

    public ApiKeyAuthMiddleware(
        RequestDelegate next,
        IConfiguration config,
        ILogger<ApiKeyAuthMiddleware> logger)
    {
        _next   = next;
        _config = config;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip auth for non-API routes (e.g. OpenAPI/Swagger)
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        // Check for header presence
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var extractedKey))
        {
            _logger.LogWarning("[AUTH] Missing API key — {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode  = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"error":"Unauthorized","message":"Missing X-Api-Key header."}""");
            return;
        }

        // Validate against configured value
        var validKey = _config["ApiKey"];
        if (!string.Equals(validKey, extractedKey, StringComparison.Ordinal))
        {
            _logger.LogWarning("[AUTH] Invalid API key attempt — {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode  = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"error":"Forbidden","message":"Invalid API key."}""");
            return;
        }

        _logger.LogInformation("[AUTH] Authenticated — {Method} {Path}",
            context.Request.Method, context.Request.Path);

        await _next(context);
    }
}

// Extension method for clean registration in Program.cs
public static class ApiKeyAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder app)
        => app.UseMiddleware<ApiKeyAuthMiddleware>();
}
