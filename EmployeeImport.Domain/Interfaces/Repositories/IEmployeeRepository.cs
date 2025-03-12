using EmployeeImport.Domain.Entities;

namespace EmployeeImport.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for working with employees
    /// </summary>
    public interface IEmployeeRepository
    {
        /// <summary>
        /// Gets all employees
        /// </summary>
        /// <returns>Collection of employees</returns>
        Task<IEnumerable<Employee>> GetAllAsync();
        
        /// <summary>
        /// Gets an employee by identifier
        /// </summary>
        /// <param name="id">Employee identifier</param>
        /// <returns>Employee</returns>
        Task<Employee> GetByIdAsync(int id);
        
        /// <summary>
        /// Adds an employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>Added employee</returns>
        Task<Employee> AddAsync(Employee employee);
        
        /// <summary>
        /// Adds a collection of employees
        /// </summary>
        /// <param name="employees">Collection of employees</param>
        /// <returns>Number of added employees</returns>
        Task<int> AddRangeAsync(IEnumerable<Employee> employees);
        
        /// <summary>
        /// Updates an employee
        /// </summary>
        /// <param name="employee">Employee</param>
        /// <returns>Updated employee</returns>
        Task<Employee> UpdateAsync(Employee employee);
        
        /// <summary>
        /// Deletes an employee
        /// </summary>
        /// <param name="id">Employee identifier</param>
        /// <returns>True if the employee is deleted, otherwise false</returns>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Gets a sorted and filtered list of employees
        /// </summary>
        /// <param name="sortField">Field to sort by</param>
        /// <param name="sortOrder">Sort order (asc/desc)</param>
        /// <param name="searchString">Search string</param>
        /// <returns>Filtered and sorted collection of employees</returns>
        Task<IEnumerable<Employee>> GetSortedAndFilteredAsync(string sortField, string sortOrder, string searchString);
        
        /// <summary>
        /// Gets an employee by payroll number
        /// </summary>
        /// <param name="payrollNumber">Payroll number</param>
        /// <returns>Employee or null if not found</returns>
        Task<Employee?> GetByPayrollNumberAsync(string payrollNumber);
    }
}
