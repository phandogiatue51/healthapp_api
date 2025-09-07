using HealthApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Try to read from environment (Render)
var renderConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Fallback to local connection string
var localConn = "Host=localhost;Port=5432;Database=HealthApp;Username=postgres;Password=123456";

var connectionString = string.IsNullOrEmpty(renderConn) ? localConn : renderConn;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "✅ HealthApp API is running");

app.Run();
