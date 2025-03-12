using System.Text.Json.Serialization;

namespace EmployeeImport.Application.DTOs
{
    /// <summary>
    /// DTO for the Employee entity
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Unique identifier for the employee
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        /// <summary>
        /// Payroll number
        /// </summary>
        [JsonPropertyName("payrollNumber")]
        public string PayrollNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// First name of the employee
        /// </summary>
        [JsonPropertyName("forenames")]
        public string Forenames { get; set; } = string.Empty;
        
        /// <summary>
        /// Surname of the employee
        /// </summary>
        [JsonPropertyName("surname")]
        public string Surname { get; set; } = string.Empty;
        
        /// <summary>
        /// Date of birth
        /// </summary>
        [JsonPropertyName("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        
        /// <summary>
        /// Telephone number
        /// </summary>
        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = string.Empty;
        
        /// <summary>
        /// Mobile phone number
        /// </summary>
        [JsonPropertyName("mobile")]
        public string Mobile { get; set; } = string.Empty;
        
        /// <summary>
        /// Address (line 1)
        /// </summary>
        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// Address (line 2)
        /// </summary>
        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = string.Empty;
        
        /// <summary>
        /// Postal code
        /// </summary>
        [JsonPropertyName("postcode")]
        public string Postcode { get; set; } = string.Empty;
        
        /// <summary>
        /// Home email
        /// </summary>
        [JsonPropertyName("emailHome")]
        public string EmailHome { get; set; } = string.Empty;
        
        /// <summary>
        /// Start date of employment
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
    }
}