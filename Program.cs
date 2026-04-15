using Coursera_Submission.Middleware;
using Coursera_Submission.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Dependency Injection: Register our user service
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 1. Logging Middleware — Processes all requests
app.UseRequestLogging();

// 2. Auth Middleware — Guards the API
app.UseApiKeyAuth();

// Map routes for controllers
app.MapControllers();

app.Run();
