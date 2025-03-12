using EmployeeImport.Application.Validators;
using EmployeeImport.Domain.Entities;
using FluentValidation.TestHelper;

namespace EmployeeImport.Test.Application
{
    [TestFixture]
    public class EmployeeValidatorTests
    {
        private EmployeeValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EmployeeValidator();
        }

        [Test]
        public void Validate_ValidEmployee_PassesValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        public void Validate_EmptyPayrollNumber_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.PayrollNumber);
        }

        [Test]
        public void Validate_NullPayrollNumber_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = null,
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.PayrollNumber);
        }

        [Test]
        public void Validate_EmptyForenames_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.Forenames);
        }

        [Test]
        public void Validate_EmptySurname_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.Surname);
        }

        [Test]
        public void Validate_FutureDateOfBirth_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = DateTime.Now.AddDays(1),
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.DateOfBirth);
        }

        [Test]
        public void Validate_DefaultDateOfBirth_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = default,
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.DateOfBirth);
        }

        [Test]
        public void Validate_FutureStartDate_PassesValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = DateTime.Now.AddDays(1)
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldNotHaveValidationErrorFor(e => e.StartDate);
        }

        [Test]
        public void Validate_DefaultStartDate_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = default
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.StartDate);
        }

        [Test]
        public void Validate_InvalidEmail_FailsValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1),
                EmailHome = "invalid-email"
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldHaveValidationErrorFor(e => e.EmailHome);
        }

        [Test]
        public void Validate_ValidEmail_PassesValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1),
                EmailHome = "john.doe@example.com"
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldNotHaveValidationErrorFor(e => e.EmailHome);
        }

        [Test]
        public void Validate_EmptyEmail_PassesValidation()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                StartDate = new DateTime(2020, 1, 1),
                EmailHome = ""
            };

            // Act
            var result = _validator.TestValidate(employee);

            // Assert
            result.ShouldNotHaveValidationErrorFor(e => e.EmailHome);
        }
    }
} 