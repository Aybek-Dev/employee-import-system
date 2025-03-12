namespace EmployeeImport.Domain.Exceptions
{
    /// <summary>
    /// Base domain exception
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }
    }
}