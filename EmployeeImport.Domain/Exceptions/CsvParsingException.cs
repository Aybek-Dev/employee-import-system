namespace EmployeeImport.Domain.Exceptions;

/// <summary>
/// Exception thrown when there is an error parsing a CSV file
/// </summary>
public class CsvParsingException : DomainException
{
    public CsvParsingException(string message) : base(message)
    {
    }
        
    public CsvParsingException(string message, Exception innerException) : base(message)
    {
    }
}