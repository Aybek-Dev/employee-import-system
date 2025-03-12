namespace EmployeeImport.Domain.Entities
{
    /// <summary>
    /// Represents an employee entity in the system
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Unique identifier for the employee
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Payroll number
        /// </summary>
        public string PayrollNumber { get; set; }
        
        /// <summary>
        /// First name of the employee
        /// </summary>
        public string Forenames { get; set; }
        
        /// <summary>
        /// Surname of the employee
        /// </summary>
        public string Surname { get; set; }
        
        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime DateOfBirth { get; set; }
        
        /// <summary>
        /// Telephone number
        /// </summary>
        public string Telephone { get; set; }
        
        /// <summary>
        /// Mobile phone number
        /// </summary>
        public string Mobile { get; set; }
        
        /// <summary>
        /// Address (line 1)
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Address (line 2)
        /// </summary>
        public string Address2 { get; set; }
        
        /// <summary>
        /// Postal code
        /// </summary>
        public string Postcode { get; set; }
        
        /// <summary>
        /// Home email
        /// </summary>
        public string EmailHome { get; set; }
        
        /// <summary>
        /// Start date of employment
        /// </summary>
        public DateTime StartDate { get; set; }
    }
}