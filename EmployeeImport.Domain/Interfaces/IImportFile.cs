namespace EmployeeImport.Domain.Interfaces;

/// <summary>
/// Interface for import file
/// </summary>
public interface IImportFile
{
    /// <summary>
    /// Gets the file name
    /// </summary>
    string FileName { get; }
    
    /// <summary>
    /// Gets the file length
    /// </summary>
    long Length { get; }
    
    /// <summary>
    /// Opens the file as a stream
    /// </summary>
    /// <returns>Stream containing the file contents</returns>
    Stream OpenReadStream();
} 