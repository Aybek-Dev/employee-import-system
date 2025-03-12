using EmployeeImport.Domain.Entities;

namespace EmployeeImport.Domain.Interfaces.Services
{
    /// <summary>
    /// Interface for employee service
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Gets an employee by identifier
        /// </summary>
        /// <param name="id">Employee identifier</param>
        /// <returns>Employee</returns>
        Task<Employee> GetEmployeeByIdAsync(int id);
        
        /// <summary>
        /// Imports employees from a CSV file
        /// </summary>
        /// <param name="file">CSV file to import</param>
        /// <returns>Import result</returns>
        Task<ImportResult> ImportEmployeesFromCsvAsync(IImportFile file);
        
        /// <summary>
        /// Imports employees from a stream
        /// </summary>
        /// <param name="fileStream">File stream</param>
        /// <returns>Import result</returns>
        Task<ImportResult> ImportEmployeesFromCsvAsync(Stream fileStream);
        
        /// <summary>
        /// Updates an employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>Updated employee</returns>
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        
        /// <summary>
        /// Deletes an employee
        /// </summary>
        /// <param name="id">Employee identifier</param>
        /// <returns>True if the employee is deleted, otherwise false</returns>
        Task<bool> DeleteEmployeeAsync(int id);
        
        /// <summary>
        /// Gets a sorted and filtered list of employees
        /// </summary>
        /// <param name="sortField">Field to sort by</param>
        /// <param name="sortOrder">Sort order (asc/desc)</param>
        /// <param name="searchString">Search string</param>
        /// <returns>Filtered and sorted collection of employees</returns>
        Task<IEnumerable<Employee>> GetSortedAndFilteredEmployeesAsync(string sortField, string sortOrder, string searchString);
    }
}
