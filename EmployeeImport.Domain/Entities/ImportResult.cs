namespace EmployeeImport.Domain.Entities
{
    /// <summary>
    /// Employee import result
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// Indicates if the import was successful
        /// </summary>
        public bool Success => SuccessCount > 0 && ErrorCount == 0;
        
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
        public List<Employee> ImportedEmployees { get; set; } = new List<Employee>();
    }
}