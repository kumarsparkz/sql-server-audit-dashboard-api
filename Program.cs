using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using AuditDashboard.Data;
using AuditDashboard.Services;
using AuditDashboard.Hubs;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Server=(localdb)\\mssqllocaldb;Database=AuditDashboard;Trusted_Connection=true;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AuditDashboardContext>(options =>
    options.UseSqlServer(connectionString));

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();
builder.Services.AddHostedService<MetricCollectionService>();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline - Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Audit Dashboard API V1");
    c.RoutePrefix = "swagger";
});

// Use CORS - MUST be before UseAuthorization and MapControllers
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<DashboardHub>("/dashboardHub");

// Initialize database
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuditDashboardContext>();
    
    // Try to apply migrations first, if that fails, use EnsureCreated
    try
    {
        await context.Database.MigrateAsync();
        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception migrationEx)
    {
        Console.WriteLine($"Database migration error: {migrationEx.Message}");
        
        // Fallback to EnsureCreated
        if (await context.Database.EnsureCreatedAsync())
        {
            Console.WriteLine("Database created with EnsureCreated fallback.");
        }
    }

    // Initialize admin user
    await InitializeAdminUser(context);
    
    Console.WriteLine("Database initialization completed.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during database initialization: {ex.Message}");
}

Console.WriteLine("🚀 API Server Started!");
Console.WriteLine("📖 Swagger UI: http://localhost:7001/swagger");
Console.WriteLine("🌐 Dashboard API: http://localhost:7001/api/dashboard/overview");
Console.WriteLine("📊 Servers API: http://localhost:7001/api/dashboard/servers");
Console.WriteLine("🧪 Test API: http://localhost:7001/api/test");

app.Run();

async Task InitializeAdminUser(AuditDashboardContext context)
{
    try
    {
        var adminExists = await context.DashboardUsers.AnyAsync(u => u.Username == "admin");
        if (!adminExists)
        {
            var adminUser = new AuditDashboard.Models.DashboardUser
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@company.com",
                Role = "Administrator",
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastLoginDate = null
            };

            context.DashboardUsers.Add(adminUser);
            await context.SaveChangesAsync();
            Console.WriteLine("Admin user created successfully.");
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating admin user: {ex.Message}");
    }
}