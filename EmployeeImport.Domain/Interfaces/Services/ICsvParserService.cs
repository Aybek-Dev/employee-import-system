using EmployeeImport.Domain.Entities;

namespace EmployeeImport.Domain.Interfaces.Services
{
    /// <summary>
    /// Service interface for parsing CSV files
    /// </summary>
    public interface ICsvParserService
    {
        /// <summary>
        /// Parses a CSV file and returns a collection of employees
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <returns>Collection of employees</returns>
        Task<IEnumerable<Employee>> ParseCsvFileAsync(Stream fileStream);
    }
}