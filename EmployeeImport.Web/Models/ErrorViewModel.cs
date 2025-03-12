namespace EmployeeImport.Web.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
} 