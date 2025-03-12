namespace EmployeeImport.Domain.Exceptions;

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
        
    public NotFoundException(string entityName, object key) 
        : base($"Entity '{entityName}' with key '{key}' was not found.")
    {
    }
}