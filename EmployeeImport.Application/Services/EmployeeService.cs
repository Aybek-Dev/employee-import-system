using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Domain.Interfaces;
using EmployeeImport.Domain.Interfaces.Repositories;
using EmployeeImport.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Application.Services
{
    /// <summary>
    /// Service for managing employees
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICsvParserService _csvParserService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository, 
            ICsvParserService csvParserService,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _csvParserService = csvParserService;
            _logger = logger;
        }

        public async Task<ImportResult> ImportEmployeesFromCsvAsync(IImportFile file)
        {
            var result = new ImportResult();

            try
            {
                await using var stream = file.OpenReadStream();
                return await ImportEmployeesFromCsvAsync(stream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing employees from CSV file");
                result.ErrorCount++;
                result.ErrorMessages.Add($"Import error: {ex.Message}");
                return result;
            }
        }

        public async Task<ImportResult> ImportEmployeesFromCsvAsync(Stream fileStream)
        {
            var result = new ImportResult();

            try
            {
                // Parse CSV file
                var employees = await _csvParserService.ParseCsvFileAsync(fileStream);
                
                if (employees == null || !employees.Any())
                {
                    result.ErrorCount++;
                    result.ErrorMessages.Add("The file contains no data or has an invalid format");
                    return result;
                }

                // Add all employees, ignoring duplicate checks
                var successCount = await _employeeRepository.AddRangeAsync(employees);
                
                result.SuccessCount = successCount;
                result.ImportedEmployees = employees.ToList();
                
                _logger.LogInformation($"Successfully imported {successCount} employees");
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing employees from CSV file");
                result.ErrorCount++;
                result.ErrorMessages.Add($"Import error: {ex.Message}");
                return result;
            }
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                if (employee == null)
                {
                    throw new ArgumentNullException(nameof(employee));
                }

                // Check if the employee exists
                var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
                if (existingEmployee == null)
                {
                    throw new NotFoundException($"Employee with ID {employee.Id} not found");
                }

                // Update employee
                return await _employeeRepository.UpdateAsync(employee);
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ArgumentNullException))
            {
                _logger.LogError(ex, $"Error updating employee with ID {employee?.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                // Check if the employee exists
                var existingEmployee = await _employeeRepository.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    throw new NotFoundException($"Employee with ID {id} not found");
                }

                // Delete employee
                return await _employeeRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}");
                throw;
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    throw new NotFoundException($"Employee with ID {id} not found");
                }
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving employee with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetSortedAndFilteredEmployeesAsync(string sortField, string sortOrder, string searchString)
        {
            try
            {
                return await _employeeRepository.GetSortedAndFilteredAsync(sortField, sortOrder, searchString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sorted and filtered employee list");
                throw;
            }
        }
    }
}
