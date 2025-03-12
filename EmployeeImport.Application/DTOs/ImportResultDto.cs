namespace EmployeeImport.Application.DTOs
{
    /// <summary>
    /// DTO for the import result
    /// </summary>
    public class ImportResultDto
    {
        /// <summary>
        /// Indicates if the import was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Number of successfully imported records
        /// </summary>
        public int SuccessCount { get; set; }
        
        /// <summary>
        /// Number of records with errors
        /// </summary>
        public int ErrorCount { get; set; }
        
        /// <summary>
        /// Error messages
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();
        
        /// <summary>
        /// Imported employees
        /// </summary>
        public List<EmployeeDto> ImportedEmployees { get; set; } = new List<EmployeeDto>();
    }
}