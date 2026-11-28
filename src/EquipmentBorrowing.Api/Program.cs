using EquipmentBorrowing.Application.Interfaces;
using EquipmentBorrowing.Application.Services;
using EquipmentBorrowing.Infrastructure.Data;
using EquipmentBorrowing.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─── Controllers + JSON ───────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ─── OpenAPI (machine-readable at /openapi/v1.json) ──────────────────────────
builder.Services.AddOpenApi("v1");

// ─── Database ─────────────────────────────────────────────────────────────────
var connectionString =
    builder.Configuration["EQUIPMENT_BORROWING_CONNECTION"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "No connection string found. Set EQUIPMENT_BORROWING_CONNECTION " +
        "or ConnectionStrings:DefaultConnection in appsettings.json.");

builder.Services.AddDbContext<EquipmentBorrowingDbContext>(options =>
    options.UseNpgsql(connectionString));

// ─── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IStudentRepository, EfStudentRepository>();
builder.Services.AddScoped<IEquipmentRepository, EfEquipmentRepository>();
builder.Services.AddScoped<IBorrowingRepository, EfBorrowingRepository>();

// ─── Application Services ─────────────────────────────────────────────────────
builder.Services.AddScoped<RegisterStudentService>();
builder.Services.AddScoped<UpdateStudentInformationService>();
builder.Services.AddScoped<ChangeStudentEligibilityService>();
builder.Services.AddScoped<ViewStudentBorrowingHistoryService>();
builder.Services.AddScoped<RegisterEquipmentService>();
builder.Services.AddScoped<UpdateEquipmentInformationService>();
builder.Services.AddScoped<ViewEquipmentCatalogService>();
builder.Services.AddScoped<CheckAvailableEquipmentService>();
builder.Services.AddScoped<ViewEquipmentBorrowingHistoryService>();
builder.Services.AddScoped<BorrowEquipmentService>();
builder.Services.AddScoped<ReturnEquipmentService>();
builder.Services.AddScoped<SearchBorrowingsService>();

var app = builder.Build();

// ─── Safe Global Error Handler (T08: No Stack Traces or Secrets Leaked) ───────
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            var error = new EquipmentBorrowing.Api.Dtos.ErrorResponse(
                "INTERNAL_SERVER_ERROR",
                "An unexpected error occurred while processing your request.");
            await context.Response.WriteAsJsonAsync(error);
        }
    }
});

// ─── Middleware ────────────────────────────────────────────────────────────────
app.MapOpenApi();
app.UseRouting();
app.MapControllers();

app.Run();

// Exposes the top-level application type to the integration-test host.
public partial class Program;
