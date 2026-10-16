using Microsoft.EntityFrameworkCore;
using Piping.Api.Api;
using Piping.Api.Application.Contracts;
using Piping.Api.Application.Services;
using Piping.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
var databasePath = Path.Combine(builder.Environment.ContentRootPath, "piping.db");
var configuredConnection = builder.Configuration.GetConnectionString("Piping");
var connectionString = string.IsNullOrWhiteSpace(configuredConnection) ? $"Data Source={databasePath}" : configuredConnection;
builder.Services.AddDbContext<PipingDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<PipingRepository>();
builder.Services.AddScoped<IPipingLineRepository>(services => services.GetRequiredService<PipingRepository>());
builder.Services.AddScoped<ICmlRepository>(services => services.GetRequiredService<PipingRepository>());
builder.Services.AddScoped<ITestPointRepository>(services => services.GetRequiredService<PipingRepository>());
builder.Services.AddScoped<IThicknessReadingRepository>(services => services.GetRequiredService<PipingRepository>());
builder.Services.AddScoped<IPipeCalculationRepository>(services => services.GetRequiredService<PipingRepository>());
builder.Services.AddScoped<IPipeCalculationService, PipeCalculationService>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DatabaseConstraintExceptionHandler>();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseExceptionHandler();
app.UseCors("Frontend");
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<PipingDbContext>();
    await db.Database.MigrateAsync();
}
await DatabaseInitializer.SeedReferenceDataAsync(app.Services);
app.Run();

public partial class Program;
