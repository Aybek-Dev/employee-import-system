using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Infrastructure.Data;
using EmployeeImport.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeImport.Test.Infrastructure
{
    [TestFixture]
    public class EmployeeRepositoryTests
    {
        private ApplicationDbContext _context;
        private EmployeeRepository _repository;
        private Mock<ILogger<EmployeeRepository>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"EmployeeTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<EmployeeRepository>>();
            _repository = new EmployeeRepository(_context, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllEmployees()
        {
            // Arrange
            await SeedTestData();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsEmployee()
        {
            // Arrange
            var employee = await SeedSingleEmployee();

            // Act
            var result = await _repository.GetByIdAsync(employee.Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(employee.Id));
            Assert.That(result.PayrollNumber, Is.EqualTo(employee.PayrollNumber));
            Assert.That(result.Forenames, Is.EqualTo(employee.Forenames));
            Assert.That(result.Surname, Is.EqualTo(employee.Surname));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var nonExistingId = 999;

            // Act
            var result = await _repository.GetByIdAsync(nonExistingId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByPayrollNumberAsync_ExistingPayrollNumber_ReturnsEmployee()
        {
            // Arrange
            var employee = await SeedSingleEmployee();

            // Act
            var result = await _repository.GetByPayrollNumberAsync(employee.PayrollNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PayrollNumber, Is.EqualTo(employee.PayrollNumber));
        }

        [Test]
        public async Task GetByPayrollNumberAsync_NonExistingPayrollNumber_ReturnsNull()
        {
            // Arrange
            var nonExistingPayrollNumber = "NONEXISTENT";

            // Act
            var result = await _repository.GetByPayrollNumberAsync(nonExistingPayrollNumber);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddAsync_ValidEmployee_AddsToDatabase()
        {
            // Arrange
            var employee = new Employee
            {
                PayrollNumber = "EMP123",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Telephone = "1234567890",
                Mobile = "0987654321",
                Address = "123 Main St",
                Address2 = "Apt 4B",
                Postcode = "12345",
                EmailHome = "john.doe@example.com",
                StartDate = new DateTime(2020, 1, 1)
            };

            // Act
            var result = await _repository.AddAsync(employee);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            
            // Verify it's in the database
            var savedEmployee = await _context.Employees.FindAsync(result.Id);
            Assert.That(savedEmployee, Is.Not.Null);
            Assert.That(savedEmployee.PayrollNumber, Is.EqualTo("EMP123"));
            Assert.That(savedEmployee.Forenames, Is.EqualTo("John"));
            Assert.That(savedEmployee.Surname, Is.EqualTo("Doe"));
        }

        [Test]
        public async Task AddRangeAsync_MultipleEmployees_AddsAllToDatabase()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee
                {
                    PayrollNumber = "EMP123",
                    Forenames = "John",
                    Surname = "Doe",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    StartDate = new DateTime(2020, 1, 1),
                    Address = "123 Main St",
                    Address2 = "Apt 4B",
                    EmailHome = "john.doe@example.com",
                    Mobile = "123-456-7890",
                    Postcode = "12345",
                    Telephone = "987-654-3210"
                },
                new Employee
                {
                    PayrollNumber = "EMP456",
                    Forenames = "Jane",
                    Surname = "Smith",
                    DateOfBirth = new DateTime(1985, 5, 5),
                    StartDate = new DateTime(2021, 2, 2),
                    Address = "456 Elm St",
                    Address2 = "Apt 5C",
                    EmailHome = "jane.smith@example.com",
                    Mobile = "234-567-8901",
                    Postcode = "67890",
                    Telephone = "876-543-2109"
                }
            };

            // Act
            await _repository.AddRangeAsync(employees);

            // Assert
            var savedEmployees = await _context.Employees.ToListAsync();
            Assert.That(savedEmployees, Is.Not.Null);
            Assert.That(savedEmployees.Count, Is.EqualTo(2));
            Assert.That(savedEmployees.Any(e => e.PayrollNumber == "EMP123"), Is.True);
            Assert.That(savedEmployees.Any(e => e.PayrollNumber == "EMP456"), Is.True);
        }

        [Test]
        public async Task UpdateAsync_ExistingEmployee_UpdatesInDatabase()
        {
            // Arrange
            var employee = await SeedSingleEmployee();
            
            // Modify the employee
            employee.Forenames = "Updated Name";
            employee.Telephone = "9999999999";

            // Act
            await _repository.UpdateAsync(employee);

            // Assert
            var updatedEmployee = await _context.Employees.FindAsync(employee.Id);
            Assert.That(updatedEmployee, Is.Not.Null);
            Assert.That(updatedEmployee.Forenames, Is.EqualTo("Updated Name"));
            Assert.That(updatedEmployee.Telephone, Is.EqualTo("9999999999"));
        }

        [Test]
        public void UpdateAsync_NonExistingEmployee_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistingEmployee = new Employee
            {
                Id = 999,
                PayrollNumber = "NONEXISTENT",
                Forenames = "Non",
                Surname = "Existent"
            };

            // Act & Assert
            var exception = Assert.ThrowsAsync<NotFoundException>(async () => 
                await _repository.UpdateAsync(nonExistingEmployee));
            
            Assert.That(exception.Message, Does.Contain("Employee with ID 999 not found"));
        }

        [Test]
        public async Task DeleteAsync_ExistingEmployee_RemovesFromDatabase()
        {
            // Arrange
            var employee = await SeedSingleEmployee();

            // Act
            var result = await _repository.DeleteAsync(employee.Id);

            // Assert
            Assert.That(result, Is.True);
            
            var deletedEmployee = await _context.Employees.FindAsync(employee.Id);
            Assert.That(deletedEmployee, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_NonExistingEmployee_ReturnsFalse()
        {
            // Arrange
            var nonExistingId = 999;

            // Act
            var result = await _repository.DeleteAsync(nonExistingId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetSortedAndFilteredAsync_WithSearchString_ReturnsFilteredEmployees()
        {
            // Arrange
            await SeedTestData();
            var searchString = "John";

            // Act
            var result = await _repository.GetSortedAndFilteredAsync("surname","asc", searchString);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Forenames, Is.EqualTo("John"));
        }

        [Test]
        public async Task GetSortedAndFilteredAsync_WithSortFieldAndOrder_ReturnsSortedEmployees()
        {
            // Arrange
            await SeedTestData();
            
            // Act - Sort by surname ascending
            var resultAsc = await _repository.GetSortedAndFilteredAsync("Surname", "asc", null);
            
            // Assert
            Assert.That(resultAsc, Is.Not.Null);
            Assert.That(resultAsc.Count(), Is.EqualTo(3));
            Assert.That(resultAsc.First().Surname, Is.EqualTo("Doe"));
            Assert.That(resultAsc.Last().Surname, Is.EqualTo("Williams"));
            
            // Act - Sort by surname descending
            var resultDesc = await _repository.GetSortedAndFilteredAsync("Surname", "desc", null);
            
            // Assert
            Assert.That(resultDesc, Is.Not.Null);
            Assert.That(resultDesc.Count(), Is.EqualTo(3));
            Assert.That(resultDesc.First().Surname, Is.EqualTo("Williams"));
            Assert.That(resultDesc.Last().Surname, Is.EqualTo("Doe"));
        }

        private async Task SeedTestData()
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    PayrollNumber = "EMP001",
                    Forenames = "John",
                    Surname = "Doe",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Telephone = "1234567890",
                    Mobile = "0987654321",
                    Address = "123 Main St",
                    Address2 = "Apt 4B",
                    Postcode = "12345",
                    EmailHome = "john.doe@example.com",
                    StartDate = new DateTime(2020, 1, 1)
                },
                new Employee
                {
                    PayrollNumber = "EMP002",
                    Forenames = "Jane",
                    Surname = "Smith",
                    DateOfBirth = new DateTime(1985, 5, 5),
                    Telephone = "2345678901",
                    Mobile = "1098765432",
                    Address = "456 Oak Ave",
                    Address2 = "Suite 7C",
                    Postcode = "23456",
                    EmailHome = "jane.smith@example.com",
                    StartDate = new DateTime(2021, 2, 2)
                },
                new Employee
                {
                    PayrollNumber = "EMP003",
                    Forenames = "Robert",
                    Surname = "Williams",
                    DateOfBirth = new DateTime(1990, 10, 10),
                    Telephone = "3456789012",
                    Mobile = "2109876543",
                    Address = "789 Pine Rd",
                    Address2 = "Unit 10D",
                    Postcode = "34567",
                    EmailHome = "robert.williams@example.com",
                    StartDate = new DateTime(2022, 3, 3)
                }
            };

            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();
        }

        private async Task<Employee> SeedSingleEmployee()
        {
            var employee = new Employee
            {
                PayrollNumber = "EMP001",
                Forenames = "John",
                Surname = "Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Telephone = "1234567890",
                Mobile = "0987654321",
                Address = "123 Main St",
                Address2 = "Apt 4B",
                Postcode = "12345",
                EmailHome = "john.doe@example.com",
                StartDate = new DateTime(2020, 1, 1)
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }
    }
} 