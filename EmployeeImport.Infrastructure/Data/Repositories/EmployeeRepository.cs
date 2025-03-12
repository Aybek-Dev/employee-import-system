using System.Transactions;
using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository for managing employee data in the database
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeRepository> _logger;
        
        public EmployeeRepository(ApplicationDbContext context, ILogger<EmployeeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        /// <summary>
        /// Retrieves all employees from the database
        /// </summary>
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }
        
        /// <summary>
        /// Retrieves an employee by their ID
        /// </summary>
        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees.FindAsync(id);
        }
        
        /// <summary>
        /// Retrieves an employee by their payroll number
        /// </summary>
        public async Task<Employee?> GetByPayrollNumberAsync(string payrollNumber)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.PayrollNumber == payrollNumber);
        }
        
        /// <summary>
        /// Adds a new employee to the database
        /// </summary>
        public async Task<Employee> AddAsync(Employee employee)
        {
            try
            {
                // Set Id to 0 to ensure Entity Framework creates a new record
                employee.Id = 0;
                
                // Detach entity if it's already being tracked
                var entry = _context.Entry(employee);
                if (entry.State != EntityState.Detached)
                {
                    entry.State = EntityState.Detached;
                }
                
                // Create a new employee instance
                var newEmployee = new Employee
                {
                    PayrollNumber = employee.PayrollNumber,
                    Forenames = employee.Forenames,
                    Surname = employee.Surname,
                    DateOfBirth = employee.DateOfBirth,
                    Telephone = employee.Telephone,
                    Mobile = employee.Mobile,
                    Address = employee.Address,
                    Address2 = employee.Address2,
                    Postcode = employee.Postcode,
                    EmailHome = employee.EmailHome,
                    StartDate = employee.StartDate
                };
                
                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully added employee: {PayrollNumber} - {Name}", 
                    newEmployee.PayrollNumber, $"{newEmployee.Forenames} {newEmployee.Surname}");
                
                return newEmployee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee {PayrollNumber}: {Message}", 
                    employee.PayrollNumber, ex.Message);
                return employee;
            }
        }
        
        /// <summary>
        /// Adds multiple employees to the database
        /// </summary>
        public async Task<int> AddRangeAsync(IEnumerable<Employee> employees)
        {
            int successCount = 0;
            
            using (var scope = new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                foreach (var employee in employees)
                {
                    try
                    {
                        var newEmployee = new Employee
                        {
                            PayrollNumber = employee.PayrollNumber,
                            Forenames = employee.Forenames,
                            Surname = employee.Surname,
                            DateOfBirth = employee.DateOfBirth,
                            Telephone = employee.Telephone,
                            Mobile = employee.Mobile,
                            Address = employee.Address,
                            Address2 = employee.Address2,
                            Postcode = employee.Postcode,
                            EmailHome = employee.EmailHome,
                            StartDate = employee.StartDate
                        };
                        
                        _context.Employees.Add(newEmployee);
                        await _context.SaveChangesAsync();
                        
                        _context.ChangeTracker.Clear();
                        
                        _logger.LogInformation("Successfully added employee: {PayrollNumber} - {Name}", 
                            newEmployee.PayrollNumber, $"{newEmployee.Forenames} {newEmployee.Surname}");
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error adding employee {PayrollNumber}: {Message}", 
                            employee.PayrollNumber, ex.Message);
                    }
                }
                
                scope.Complete();
            }
            
            return successCount;
        }
        
        /// <summary>
        /// Updates an existing employee in the database
        /// </summary>
        public async Task<Employee> UpdateAsync(Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.Id);
            
            if (existingEmployee == null)
            {
                throw new NotFoundException($"Employee with ID {employee.Id} not found");
            }
            
            _context.Entry(existingEmployee).State = EntityState.Detached;
            
            _context.Employees.Attach(employee);
            _context.Entry(employee).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully updated employee: {PayrollNumber} - {Name}", 
                employee.PayrollNumber, $"{employee.Forenames} {employee.Surname}");
            
            return employee;
        }
        
        /// <summary>
        /// Deletes an employee from the database
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            
            if (employee == null)
            {
                _logger.LogWarning("Attempted to delete non-existent employee with ID: {Id}", id);
                return false;
            }
            
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully deleted employee: {PayrollNumber} - {Name}", 
                employee.PayrollNumber, $"{employee.Forenames} {employee.Surname}");
            
            return true;
        }
        
        /// <summary>
        /// Retrieves sorted and filtered employees based on search criteria
        /// </summary>
        public async Task<IEnumerable<Employee>> GetSortedAndFilteredAsync(string sortField, string sortOrder, string searchString)
        {
            var query = _context.Employees.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                
                query = query.Where(e => 
                    e.PayrollNumber.ToLower().Contains(searchString) ||
                    e.Forenames.ToLower().Contains(searchString) ||
                    e.Surname.ToLower().Contains(searchString) ||
                    e.EmailHome.ToLower().Contains(searchString) ||
                    e.Address.ToLower().Contains(searchString) ||
                    e.Address2.ToLower().Contains(searchString) ||
                    e.Postcode.ToLower().Contains(searchString)
                );
            }
            query = sortOrder.Equals("desc"
, StringComparison.CurrentCultureIgnoreCase)
                ? ApplyDescendingSort(query, sortField)
                : ApplyAscendingSort(query, sortField);
            
            return await query.ToListAsync();
        }
        
        /// <summary>
        /// Applies ascending sort to the query based on the specified field
        /// </summary>
        private IQueryable<Employee> ApplyAscendingSort(IQueryable<Employee> query, string sortField)
        {
            return sortField.ToLower() switch
            {
                "id" => query.OrderBy(e => e.Id),
                "payrollnumber" => query.OrderBy(e => e.PayrollNumber),
                "forenames" => query.OrderBy(e => e.Forenames),
                "surname" => query.OrderBy(e => e.Surname),
                "dateofbirth" => query.OrderBy(e => e.DateOfBirth),
                "telephone" => query.OrderBy(e => e.Telephone),
                "mobile" => query.OrderBy(e => e.Mobile),
                "address" => query.OrderBy(e => e.Address),
                "address2" => query.OrderBy(e => e.Address2),
                "postcode" => query.OrderBy(e => e.Postcode),
                "emailhome" => query.OrderBy(e => e.EmailHome),
                "startdate" => query.OrderBy(e => e.StartDate),
                _ => query.OrderBy(e => e.Surname) // Default sort by surname
            };
        }
        
        /// <summary>
        /// Applies descending sort to the query based on the specified field
        /// </summary>
        private IQueryable<Employee> ApplyDescendingSort(IQueryable<Employee> query, string sortField)
        {
            return sortField.ToLower() switch
            {
                "id" => query.OrderByDescending(e => e.Id),
                "payrollnumber" => query.OrderByDescending(e => e.PayrollNumber),
                "forenames" => query.OrderByDescending(e => e.Forenames),
                "surname" => query.OrderByDescending(e => e.Surname),
                "dateofbirth" => query.OrderByDescending(e => e.DateOfBirth),
                "telephone" => query.OrderByDescending(e => e.Telephone),
                "mobile" => query.OrderByDescending(e => e.Mobile),
                "address" => query.OrderByDescending(e => e.Address),
                "address2" => query.OrderByDescending(e => e.Address2),
                "postcode" => query.OrderByDescending(e => e.Postcode),
                "emailhome" => query.OrderByDescending(e => e.EmailHome),
                "startdate" => query.OrderByDescending(e => e.StartDate),
                _ => query.OrderByDescending(e => e.Surname) // Default sort by surname
            };
        }
    }
}
