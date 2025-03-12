using EmployeeImport.Domain.Entities;
using FluentValidation;

namespace EmployeeImport.Application.Validators
{
    /// <summary>
    /// Validator for the Employee entity
    /// </summary>
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.PayrollNumber)
                .NotEmpty().WithMessage("Payroll number is required")
                .MaximumLength(50).WithMessage("Payroll number must not exceed 50 characters");
                
            RuleFor(e => e.Forenames)
                .NotEmpty().WithMessage("Forename is required")
                .MaximumLength(100).WithMessage("Forename must not exceed 100 characters");
                
            RuleFor(e => e.Surname)
                .NotEmpty().WithMessage("Surname is required")
                .MaximumLength(100).WithMessage("Surname must not exceed 100 characters");
                
            RuleFor(e => e.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");
                
            RuleFor(e => e.Telephone)
                .MaximumLength(20).WithMessage("Telephone number must not exceed 20 characters");
                
            RuleFor(e => e.Mobile)
                .MaximumLength(20).WithMessage("Mobile phone number must not exceed 20 characters");
                
            RuleFor(e => e.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters");
                
            RuleFor(e => e.Address2)
                .MaximumLength(200).WithMessage("Address line 2 must not exceed 200 characters");
                
            RuleFor(e => e.Postcode)
                .MaximumLength(20).WithMessage("Postcode must not exceed 20 characters");
                
            RuleFor(e => e.EmailHome)
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
                .EmailAddress().When(e => !string.IsNullOrEmpty(e.EmailHome)).WithMessage("Invalid email format");
                
            RuleFor(e => e.StartDate)
                .NotEmpty().WithMessage("Start date is required");
        }
    }
}
