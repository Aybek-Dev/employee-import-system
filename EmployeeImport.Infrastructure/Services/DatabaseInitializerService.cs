using System.Threading.Tasks;
using EmployeeImport.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for initializing the database
    /// </summary>
    public class DatabaseInitializerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseInitializerService> _logger;

        public DatabaseInitializerService(
            ApplicationDbContext context,
            ILogger<DatabaseInitializerService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database by checking for and applying migrations
        /// </summary>
        public async Task InitializeDatabaseAsync()
        {
            _logger.LogInformation("Initializing database...");

            try
            {
                // Применяем все миграции (если их нет - просто пропускается)
                await _context.Database.MigrateAsync();
        
                _logger.LogInformation("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

    }
} 