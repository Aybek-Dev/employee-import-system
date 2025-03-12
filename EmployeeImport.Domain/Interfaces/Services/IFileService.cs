namespace EmployeeImport.Domain.Interfaces.Services;

/// <summary>
/// Interface for file service
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Checks if the file is a CSV file
    /// </summary>
    /// <param name="file">File to check</param>
    /// <returns>True if the file is a CSV file, false otherwise</returns>
    bool IsCsvFile(IImportFile file);
    
    /// <summary>
    /// Gets a stream for the file
    /// </summary>
    /// <param name="file">File to get stream for</param>
    /// <returns>Stream containing the file contents</returns>
    Task<Stream> GetFileStreamAsync(IImportFile file);
} 