using AutoMapper;
using EmployeeImport.Application.DTOs;
using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Domain.Interfaces.Services;
using EmployeeImport.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeImport.Web.Controllers
{
    /// <summary>
    /// Controller for working with employees
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, IMapper mapper, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Main page
        /// </summary>
        /// <param name="searchString">Search string</param>
        /// <returns>View</returns>
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                var employees = await _employeeService.GetSortedAndFilteredEmployeesAsync("Surname", "asc", searchString);
                var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
                
                // Convert to a format with id in lowercase for DevExtreme
                var formattedEmployees = employeeDtos.Select(e => new
                {
                    id = e.Id,
                    payrollNumber = e.PayrollNumber,
                    forenames = e.Forenames,
                    surname = e.Surname,
                    dateOfBirth = e.DateOfBirth,
                    telephone = e.Telephone,
                    mobile = e.Mobile,
                    address = e.Address,
                    address2 = e.Address2,
                    postcode = e.Postcode,
                    emailHome = e.EmailHome,
                    startDate = e.StartDate
                }).ToList();
                
                ViewBag.Employees = formattedEmployees;
                ViewBag.SearchString = searchString;
                return View(new ImportViewModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading the employee list");
                return View("Error", new ErrorViewModel 
                { 
                    ErrorMessage = "Error loading the employee list. Please try again later.",
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        /// <summary>
        /// Import employees from CSV file
        /// </summary>
        /// <param name="model">Import model</param>
        /// <returns>JSON result</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import([FromForm] ImportViewModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    return Json(new { success = false, message = "Please select a file" });
                }

                if (!model.File.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new { success = false, message = "Please select a CSV file" });
                }

                // Adapter for IFormFile -> IImportFile
                var fileAdapter = new FormFileAdapter(model.File);
                
                // Import employees
                var result = await _employeeService.ImportEmployeesFromCsvAsync(fileAdapter);
                
                if (result.Success)
                {
                    // Get the updated list of employees
                    var employees = await _employeeService.GetSortedAndFilteredEmployeesAsync("Surname", "asc", string.Empty);
                    var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
                    
                    // Convert to a format with id in lowercase for DevExtreme
                    var formattedEmployees = employeeDtos.Select(e => new
                    {
                        id = e.Id,
                        payrollNumber = e.PayrollNumber,
                        forenames = e.Forenames,
                        surname = e.Surname,
                        dateOfBirth = e.DateOfBirth,
                        telephone = e.Telephone,
                        mobile = e.Mobile,
                        address = e.Address,
                        address2 = e.Address2,
                        postcode = e.Postcode,
                        emailHome = e.EmailHome,
                        startDate = e.StartDate
                    }).ToList();
                    
                    return Json(new { 
                        success = true, 
                        data = formattedEmployees,
                        successCount = result.SuccessCount,
                        errorCount = result.ErrorCount,
                        errorMessages = result.ErrorMessages
                    });
                }
                
                return Json(new { success = false, message = "Error during import: " + string.Join(", ", result.ErrorMessages) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during employee import");
                return Json(new { success = false, message = "Error during employee import" });
            }
        }

        /// <summary>
        /// Update employee
        /// </summary>
        /// <param name="employeeDto">Employee DTO</param>
        /// <returns>JSON result</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromForm] EmployeeDto employeeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Invalid data" });
                }

                try
                {
                    var employee = _mapper.Map<Employee>(employeeDto);
                    await _employeeService.UpdateEmployeeAsync(employee);
                    
                    // Get the updated list of employees
                    var employees = await _employeeService.GetSortedAndFilteredEmployeesAsync("Surname", "asc", string.Empty);
                    var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
                    
                    // Convert to a format with id in lowercase for DevExtreme
                    var formattedEmployees = employeeDtos.Select(e => new
                    {
                        id = e.Id,
                        payrollNumber = e.PayrollNumber,
                        forenames = e.Forenames,
                        surname = e.Surname,
                        dateOfBirth = e.DateOfBirth,
                        telephone = e.Telephone,
                        mobile = e.Mobile,
                        address = e.Address,
                        address2 = e.Address2,
                        postcode = e.Postcode,
                        emailHome = e.EmailHome,
                        startDate = e.StartDate
                    }).ToList();
                    
                    return Json(new { success = true, data = formattedEmployees });
                }
                catch (NotFoundException ex)
                {
                    _logger.LogWarning(ex, "Employee not found during update attempt");
                    return Json(new { success = false, message = ex.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during employee update");
                return Json(new { success = false, message = "Error during employee update" });
            }
        }

        /// <summary>
        /// Get employees with filtering and sorting
        /// </summary>
        /// <param name="searchString">Search string</param>
        /// <param name="sortField">Sort field</param>
        /// <param name="sortOrder">Sort order</param>
        /// <returns>JSON result</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string searchString, string sortField = "Surname", string sortOrder = "asc")
        {
            try
            {
                var employees = await _employeeService.GetSortedAndFilteredEmployeesAsync(sortField, sortOrder, searchString);
                var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
                
                // Convert to an anonymous object with explicit id in lowercase
                var result = employeeDtos.Select(e => new
                {
                    id = e.Id, 
                    payrollNumber = e.PayrollNumber,
                    forenames = e.Forenames,
                    surname = e.Surname,
                    dateOfBirth = e.DateOfBirth,
                    telephone = e.Telephone,
                    mobile = e.Mobile,
                    address = e.Address,
                    address2 = e.Address2,
                    postcode = e.Postcode,
                    emailHome = e.EmailHome,
                    startDate = e.StartDate
                }).ToList();
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting the employee list");
                return StatusCode(500, "Error while getting the employee list");
            }
        }

        /// <summary>
        /// Delete employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>JSON result</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Check if the employee exists
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return Json(new { success = false, message = $"Employee with ID {id} not found" });
                }
                
                // Delete the employee
                var success = await _employeeService.DeleteEmployeeAsync(id);
                
                if (success)
                {
                    // Get the updated list of employees
                    var employees = await _employeeService.GetSortedAndFilteredEmployeesAsync("Surname", "asc", string.Empty);
                    var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDto>(e)).ToList();
                    
                    // Convert to a format with id in lowercase for DevExtreme
                    var formattedEmployees = employeeDtos.Select(e => new
                    {
                        id = e.Id,
                        payrollNumber = e.PayrollNumber,
                        forenames = e.Forenames,
                        surname = e.Surname,
                        dateOfBirth = e.DateOfBirth,
                        telephone = e.Telephone,
                        mobile = e.Mobile,
                        address = e.Address,
                        address2 = e.Address2,
                        postcode = e.Postcode,
                        emailHome = e.EmailHome,
                        startDate = e.StartDate
                    }).ToList();
                    
                    return Json(new { success = true, data = formattedEmployees });
                }
                
                return Json(new { success = false, message = "Failed to delete employee" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}");
                return Json(new { success = false, message = "Error deleting employee" });
            }
        }

        /// <summary>
        /// Handles errors and displays an error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;
            
            _logger.LogError(exception, "An unhandled exception occurred");
            
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = exception?.Message ?? "An error occurred while processing your request."
            };
            
            return View(errorViewModel);
        }
    }
}
