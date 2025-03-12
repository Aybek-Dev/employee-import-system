using EmployeeImport.Domain.Interfaces;

namespace EmployeeImport.Web.Models
{
    public class FormFileAdapter : IImportFile
    {
        private readonly IFormFile _file;

        public FormFileAdapter(IFormFile file)
        {
            _file = file;
        }

        public string FileName => _file.FileName;
        public string ContentType => _file.ContentType;
        public long Length => _file.Length;

        public Stream OpenReadStream()
        {
            return _file.OpenReadStream();
        }
    }
} 