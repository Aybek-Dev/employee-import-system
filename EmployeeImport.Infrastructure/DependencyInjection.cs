using EmployeeImport.Domain.Interfaces.Repositories;
using EmployeeImport.Domain.Interfaces.Services;
using EmployeeImport.Infrastructure.Data;
using EmployeeImport.Infrastructure.Data.Repositories;
using EmployeeImport.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Infrastructure
{
    /// <summary>
    /// Class for registering infrastructure layer dependencies
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register the database context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

                // Enable detailed logging
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            });

            // Register repositories
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            // Register infrastructure services
            services.AddScoped<IFileService, FileService>();

            // Register database initializer
            services.AddScoped<DatabaseInitializerService>();

            return services;
        }
    }
}