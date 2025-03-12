using System.ComponentModel.DataAnnotations;

namespace EmployeeImport.Web.Models
{
    /// <summary>
    /// ViewModel for file import
    /// </summary>
    public class ImportViewModel
    {
        /// <summary>
        /// File to import
        /// </summary>
        [Display(Name = "CSV File")]
        public IFormFile? File { get; set; }
    }
}