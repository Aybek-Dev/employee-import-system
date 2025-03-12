using System.Globalization;
using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace EmployeeImport.Application.Services
{
    /// <summary>
    /// Service responsible for parsing CSV files containing employee data
    /// </summary>
    public class CsvParserService : ICsvParserService
    {
        private readonly string[] _requiredHeaders = new[]
        {
            "Personnel_Records.Payroll_Number",
            "Personnel_Records.Forenames",
            "Personnel_Records.Surname",
            "Personnel_Records.Date_of_Birth",
            "Personnel_Records.Start_Date"
        };

        private readonly ILogger<CsvParserService> _logger;

        public CsvParserService(ILogger<CsvParserService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Parses a CSV file stream and returns a collection of Employee entities
        /// </summary>
        public async Task<IEnumerable<Employee>> ParseCsvFileAsync(Stream fileStream)
        {
            try
            {
                var employees = new List<Employee>();
                
                // Read the entire file into memory
                string fileContent;
                using (var reader = new StreamReader(fileStream))
                {
                    fileContent = await reader.ReadToEndAsync();
                }
                
                // Log first 200 characters for diagnostics
                _logger.LogInformation("File content (first 200 chars): {Content}", 
                    fileContent.Length > 200 ? fileContent.Substring(0, 200) + "..." : fileContent);
                
                // Split file into lines
                var lines = fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length < 2)
                {
                    throw new CsvParsingException("File does not contain data or has an invalid format");
                }
                
                // Determine delimiter (comma or tab)
                var delimiter = ',';
                if (lines[0].Contains("\t"))
                {
                    delimiter = '\t';
                }
                
                _logger.LogInformation("Using delimiter: {Delimiter}", delimiter == '\t' ? "tab" : "comma");
                
                // Get headers
                var headers = lines[0].Split(delimiter);
                _logger.LogInformation("Found headers: {Headers}", string.Join(", ", headers));
                
                // Check for required headers
                var missingHeaders = _requiredHeaders.Where(h => !headers.Contains(h)).ToList();
                if (missingHeaders.Any())
                {
                    _logger.LogWarning("Missing required headers: {MissingHeaders}", string.Join(", ", missingHeaders));
                    
                    // Check case-insensitive if exact match not found
                    var headersLower = headers.Select(h => h.ToLower()).ToArray();
                    var requiredHeadersLower = _requiredHeaders.Select(h => h.ToLower()).ToArray();
                    
                    missingHeaders = requiredHeadersLower.Where(h => !headersLower.Contains(h)).ToList();
                    if (missingHeaders.Any())
                    {
                        throw new CsvParsingException($"Missing required headers: {string.Join(", ", missingHeaders)}");
                    }
                }
                
                // Process data
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        var line = lines[i];
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        
                        _logger.LogInformation("Processing line {LineNumber}: {Line}", i, line);
                        
                        // Split line into values
                        var values = line.Split(delimiter);
                        
                        // Create employee
                        var employee = new Employee();
                        
                        // Fill employee data
                        for (int j = 0; j < headers.Length && j < values.Length; j++)
                        {
                            var header = headers[j];
                            var value = values[j].Trim();
                            
                            switch (header.ToLower())
                            {
                                case "personnel_records.payroll_number":
                                    employee.PayrollNumber = value;
                                    break;
                                case "personnel_records.forenames":
                                    employee.Forenames = value;
                                    break;
                                case "personnel_records.surname":
                                    employee.Surname = value;
                                    break;
                                case "personnel_records.date_of_birth":
                                    if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
                                        employee.DateOfBirth = dob;
                                    else if (DateTime.TryParse(value, out dob))
                                        employee.DateOfBirth = dob;
                                    break;
                                case "personnel_records.telephone":
                                    employee.Telephone = value;
                                    break;
                                case "personnel_records.mobile":
                                    employee.Mobile = value;
                                    break;
                                case "personnel_records.address":
                                    employee.Address = value;
                                    break;
                                case "personnel_records.address_2":
                                    employee.Address2 = value;
                                    break;
                                case "personnel_records.postcode":
                                    employee.Postcode = value;
                                    break;
                                case "personnel_records.email_home":
                                    employee.EmailHome = value;
                                    break;
                                case "personnel_records.start_date":
                                    if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
                                        employee.StartDate = startDate;
                                    else if (DateTime.TryParse(value, out startDate))
                                        employee.StartDate = startDate;
                                    break;
                            }
                        }
                        
                        // Basic validation
                        if (string.IsNullOrWhiteSpace(employee.PayrollNumber))
                        {
                            _logger.LogWarning("Empty PayrollNumber at line {LineNumber}", i);
                            continue;
                        }
                        
                        if (employee.DateOfBirth >= DateTime.Now)
                        {
                            _logger.LogWarning("Invalid DateOfBirth at line {LineNumber}: {DateOfBirth}", i, employee.DateOfBirth);
                            continue;
                        }
                        
                        if (employee.StartDate > DateTime.Now)
                        {
                            _logger.LogWarning("Future StartDate at line {LineNumber}: {StartDate}", i, employee.StartDate);
                            continue;
                        }
                        
                        _logger.LogInformation("Successfully parsed employee: PayrollNumber={PayrollNumber}, Name={Name}", 
                            employee.PayrollNumber, $"{employee.Forenames} {employee.Surname}");
                        
                        employees.Add(employee);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing line {LineNumber}: {Line}", i, lines[i]);
                        continue;
                    }
                }
                
                _logger.LogInformation("Finished parsing CSV. Found {Count} valid employees", employees.Count);
                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV file");
                throw new CsvParsingException($"Error parsing CSV file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Detects the delimiter used in the CSV file
        /// </summary>
        private async Task<string?> DetectDelimiter(byte[] fileBytes)
        {
            using (var memStream = new MemoryStream(fileBytes))
            using (var reader = new StreamReader(memStream))
            {
                string? firstLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(firstLine))
                {
                    return null; // Empty file or read error
                }

                // Check delimiter count in first line
                int tabCount = firstLine.Count(c => c == '\t');
                int commaCount = firstLine.Count(c => c == ',');

                if (tabCount > commaCount)
                {
                    return "\t"; // Tab delimiter
                }
                else if (commaCount > tabCount)
                {
                    return ","; // Comma delimiter
                }
                else
                {
                    return null; // Could not determine delimiter
                }
            }
        }
    }
}
