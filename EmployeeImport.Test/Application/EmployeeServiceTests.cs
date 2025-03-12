using EmployeeImport.Application.Services;
using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Exceptions;
using EmployeeImport.Domain.Interfaces;
using EmployeeImport.Domain.Interfaces.Repositories;
using EmployeeImport.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeImport.Test.Application
{
    [TestFixture]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<ICsvParserService> _csvParserServiceMock;
        private Mock<ILogger<EmployeeService>> _loggerMock;
        private EmployeeService _employeeService;

        [SetUp]
        public void Setup()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _csvParserServiceMock = new Mock<ICsvParserService>();
            _loggerMock = new Mock<ILogger<EmployeeService>>();
            
            _employeeService = new EmployeeService(
                _employeeRepositoryMock.Object,
                _csvParserServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetEmployeeByIdAsync_ExistingId_ReturnsEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, PayrollNumber = "EMP001", Forenames = "John", Surname = "Doe" };
            
            _employeeRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(employee);

            // Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.PayrollNumber, Is.EqualTo("EMP001"));
            
            _employeeRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Test]
        public void GetEmployeeByIdAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            _employeeRepositoryMock.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Employee)null);

            // Act & Assert
            var exception = Assert.ThrowsAsync<NotFoundException>(async () => 
                await _employeeService.GetEmployeeByIdAsync(999));
            
            Assert.That(exception.Message, Does.Contain("Employee with ID 999 not found"));
            
            _employeeRepositoryMock.Verify(repo => repo.GetByIdAsync(999), Times.Once);
        }

        [Test]
        public async Task UpdateEmployeeAsync_ExistingEmployee_ReturnsUpdatedEmployee()
        {
            // Arrange
            var employee = new Employee { Id = 1, PayrollNumber = "EMP001", Forenames = "John", Surname = "Doe" };
            var updatedEmployee = new Employee { Id = 1, PayrollNumber = "EMP001", Forenames = "John", Surname = "Updated" };
            
            
            _employeeRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(employee);
            _employeeRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Employee>()))
                .ReturnsAsync(updatedEmployee);

            // Act
            var result = await _employeeService.UpdateEmployeeAsync(employee);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Surname, Is.EqualTo("Updated"));
            
            _employeeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Employee>()), Times.Once);
        }

        [Test] 
        public async Task DeleteEmployeeAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var employee = new Employee { Id = 1, PayrollNumber = "EMP001", Forenames = "John", Surname = "Doe" };
    
            _employeeRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(employee); 

            _employeeRepositoryMock.Setup(repo => repo.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _employeeService.DeleteEmployeeAsync(1);

            // Assert
            Assert.That(result, Is.True);

            _employeeRepositoryMock.Verify(repo => repo.GetByIdAsync(1), Times.Once); // Проверяем вызов
            _employeeRepositoryMock.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }


        [Test]
        public async Task DeleteEmployeeAsync_NonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            _employeeRepositoryMock.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((Employee)null);  // Mock to return null, meaning the employee doesn't exist

            // Act  
            var ex = Assert.ThrowsAsync<NotFoundException>(async () => 
                await _employeeService.DeleteEmployeeAsync(999));
            
            
            
            // Assert
            Assert.That(ex.Message, Is.EqualTo("Employee with ID 999 not found"));
        }



        [Test]
        public async Task GetSortedAndFilteredEmployeesAsync_CallsRepositoryWithCorrectParameters()
        {
            // Arrange
            var employees = new List<Employee>
            {
                new Employee { Id = 1, PayrollNumber = "EMP001", Forenames = "John", Surname = "Doe" },
                new Employee { Id = 2, PayrollNumber = "EMP002", Forenames = "Jane", Surname = "Smith" }
            };
            
            _employeeRepositoryMock.Setup(repo => repo.GetSortedAndFilteredAsync("Surname", "asc", "doe"))
                .ReturnsAsync(employees.Where(e => e.Surname.Contains("doe", StringComparison.OrdinalIgnoreCase)));

            // Act
            var result = await _employeeService.GetSortedAndFilteredEmployeesAsync("Surname", "asc", "doe");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Surname, Is.EqualTo("Doe"));
            
            _employeeRepositoryMock.Verify(repo => repo.GetSortedAndFilteredAsync("Surname", "asc", "doe"), Times.Once);
        }

        [Test]
        public async Task ImportEmployeesFromCsvAsync_ValidFile_ReturnsSuccessResult()
        {
            // Arrange
            var mockFile = new Mock<IImportFile>();
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            
            var employees = new List<Employee>
            {
                new Employee { PayrollNumber = "EMP001", Forenames = "John", Surname = "Doe" },
                new Employee { PayrollNumber = "EMP002", Forenames = "Jane", Surname = "Smith" }
            };
            
            _csvParserServiceMock.Setup(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()))
                .ReturnsAsync(employees);
                
            _employeeRepositoryMock.Setup(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Employee>>()))
                .ReturnsAsync(2);

            // Act
            var result = await _employeeService.ImportEmployeesFromCsvAsync(mockFile.Object);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.SuccessCount, Is.EqualTo(2));
            Assert.That(result.ErrorCount, Is.EqualTo(0));
            Assert.That(result.ErrorMessages, Is.Empty);
            
            _csvParserServiceMock.Verify(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Employee>>()), Times.Once);
        }

        [Test]
        public async Task ImportEmployeesFromCsvAsync_ParsingError_ReturnsErrorResult()
        {
            // Arrange
            var mockFile = new Mock<IImportFile>();
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            
            _csvParserServiceMock.Setup(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()))
                .ThrowsAsync(new CsvParsingException("Invalid CSV format"));

            // Act
            var result = await _employeeService.ImportEmployeesFromCsvAsync(mockFile.Object);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.SuccessCount, Is.EqualTo(0));
            Assert.That(result.ErrorCount, Is.EqualTo(1));
            Assert.That(result.ErrorMessages, Contains.Item("Import error: Invalid CSV format"));
            
            _csvParserServiceMock.Verify(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Employee>>()), Times.Never);
        }

        [Test]
        public async Task ImportEmployeesFromCsvAsync_EmptyFile_ReturnsErrorResult()
        {
            // Arrange
            var mockFile = new Mock<IImportFile>();
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            
            _csvParserServiceMock.Setup(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()))
                .ReturnsAsync(new List<Employee>());

            // Act
            var result = await _employeeService.ImportEmployeesFromCsvAsync(mockFile.Object);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.SuccessCount, Is.EqualTo(0));
            Assert.That(result.ErrorCount, Is.EqualTo(1));
            Assert.That(result.ErrorMessages, Contains.Item("The file contains no data or has an invalid format"));
            
            _csvParserServiceMock.Verify(parser => parser.ParseCsvFileAsync(It.IsAny<Stream>()), Times.Once);
            _employeeRepositoryMock.Verify(repo => repo.AddRangeAsync(It.IsAny<IEnumerable<Employee>>()), Times.Never);
        }
    }
} 