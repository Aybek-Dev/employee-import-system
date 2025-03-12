using EmployeeImport.Domain.Interfaces;
using EmployeeImport.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Infrastructure.Services
{
    /// <summary>
    /// Implementation of file service
    /// </summary>
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        
        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }
        
        /// <inheritdoc />
        public bool IsCsvFile(IImportFile file)
        {
            if (file == null)
            {
                _logger.LogWarning("File is null");
                return false;
            }
            
            var fileName = file.FileName.ToLower();
            var isCsv = fileName.EndsWith(".csv");
            
            _logger.LogInformation("Checking if file {FileName} is CSV: {IsCsv}", fileName, isCsv);
            return isCsv;
        }
        
        /// <inheritdoc />
        public async Task<Stream> GetFileStreamAsync(IImportFile file)
        {
            if (file == null)
            {
                _logger.LogWarning("File is null");
                return null;
            }
            
            if (file.Length == 0)
            {
                _logger.LogWarning("File {FileName} is empty", file.FileName);
                return null;
            }
            
            try
            {
                var stream = file.OpenReadStream();
                _logger.LogInformation("Successfully opened stream for file {FileName}", file.FileName);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening stream for file {FileName}", file.FileName);
                return null;
            }
        }
    }
}