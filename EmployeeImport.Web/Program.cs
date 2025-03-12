using EmployeeImport.Application;
using EmployeeImport.Infrastructure;
using EmployeeImport.Infrastructure.Services;
using Serilog;
using System.IO;

// Create the application builder
var builder = WebApplication.CreateBuilder(args);

// Create logs directory if it doesn't exist
var logsDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "logs");
if (!Directory.Exists(logsDirectory))
{
    Directory.CreateDirectory(logsDirectory);
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logsDirectory, "log-.txt"), rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add application services
builder.Services.AddApplicationServices();

// Add AutoMapper for Web layer
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Employee/Error");
    app.UseHsts();
}

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var databaseInitializer = services.GetRequiredService<DatabaseInitializerService>();
        await databaseInitializer.InitializeDatabaseAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
