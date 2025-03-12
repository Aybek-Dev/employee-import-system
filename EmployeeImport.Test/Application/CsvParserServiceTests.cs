using System.Text;
using EmployeeImport.Application.Services;
using EmployeeImport.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeImport.Test.Application
{
    [TestFixture]
    public class CsvParserServiceTests
    {
        private Mock<ILogger<CsvParserService>> _loggerMock;
        private CsvParserService _csvParserService;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<CsvParserService>>();
            _csvParserService = new CsvParserService(_loggerMock.Object);
        }

        [Test]
        public async Task ParseCsvFileAsync_ValidCsvWithCommaDelimiter_ReturnsEmployees()
        {
            // Arrange
            var csvContent = "Personnel_Records.Payroll_Number,Personnel_Records.Forenames,Personnel_Records.Surname,Personnel_Records.Date_of_Birth,Personnel_Records.Telephone,Personnel_Records.Mobile,Personnel_Records.Address,Personnel_Records.Address_2,Personnel_Records.Postcode,Personnel_Records.EMail_Home,Personnel_Records.Start_Date\n" +
                             "PI123,John,Doe,1980-01-01,12345,67890,123 Main St,Apt 4,AB12 3CD,john.doe@example.com,2020-01-01\n" +
                             "PI456,Jane,Smith,1985-05-05,54321,09876,456 High St,Suite 7,EF45 6GH,jane.smith@example.com,2021-02-02";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvFileAsync(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var firstEmployee = result.First();
            Assert.That(firstEmployee.PayrollNumber, Is.EqualTo("PI123"));
            Assert.That(firstEmployee.Forenames, Is.EqualTo("John"));
            Assert.That(firstEmployee.Surname, Is.EqualTo("Doe"));
            Assert.That(firstEmployee.DateOfBirth, Is.EqualTo(new DateTime(1980, 1, 1)));
            Assert.That(firstEmployee.Telephone, Is.EqualTo("12345"));
            Assert.That(firstEmployee.Mobile, Is.EqualTo("67890"));
            Assert.That(firstEmployee.Address, Is.EqualTo("123 Main St"));
            Assert.That(firstEmployee.Address2, Is.EqualTo("Apt 4"));
            Assert.That(firstEmployee.Postcode, Is.EqualTo("AB12 3CD"));
            Assert.That(firstEmployee.EmailHome, Is.EqualTo("john.doe@example.com"));
            Assert.That(firstEmployee.StartDate, Is.EqualTo(new DateTime(2020, 1, 1)));
        }

        [Test]
        public async Task ParseCsvFileAsync_ValidCsvWithTabDelimiter_ReturnsEmployees()
        {
            // Arrange
            var csvContent = "Personnel_Records.Payroll_Number\tPersonnel_Records.Forenames\tPersonnel_Records.Surname\tPersonnel_Records.Date_of_Birth\tPersonnel_Records.Telephone\tPersonnel_Records.Mobile\tPersonnel_Records.Address\tPersonnel_Records.Address_2\tPersonnel_Records.Postcode\tPersonnel_Records.EMail_Home\tPersonnel_Records.Start_Date\n" +
                             "PI123\tJohn\tDoe\t1980-01-01\t12345\t67890\t123 Main St\tApt 4\tAB12 3CD\tjohn.doe@example.com\t2020-01-01\n" +
                             "PI456\tJane\tSmith\t1985-05-05\t54321\t09876\t456 High St\tSuite 7\tEF45 6GH\tjane.smith@example.com\t2021-02-02";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvFileAsync(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));

            var secondEmployee = result.ElementAt(1);
            Assert.That(secondEmployee.PayrollNumber, Is.EqualTo("PI456"));
            Assert.That(secondEmployee.Forenames, Is.EqualTo("Jane"));
            Assert.That(secondEmployee.Surname, Is.EqualTo("Smith"));
        }

        [Test]
        public void ParseCsvFileAsync_EmptyFile_ThrowsCsvParsingException()
        {
            // Arrange
            var csvContent = "";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act & Assert
            var exception = Assert.ThrowsAsync<CsvParsingException>(async () => 
                await _csvParserService.ParseCsvFileAsync(stream));
            
            Assert.That(exception.Message, Does.Contain("Error parsing CSV file: File does not contain data or has an invalid format"));
        }

        [Test]
        public void ParseCsvFileAsync_MissingRequiredHeaders_ThrowsCsvParsingException()
        {
            // Arrange
            var csvContent = "Forenames,Surname,DateOfBirth\n" +
                             "John,Doe,1980-01-01";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act & Assert
            var exception = Assert.ThrowsAsync<CsvParsingException>(async () => 
                await _csvParserService.ParseCsvFileAsync(stream));
            
            Assert.That(exception.Message, Does.Contain("Missing required headers"));
        }

        [Test]
        public async Task ParseCsvFileAsync_EmptyPayrollNumber_SkipsRecord()
        {
            // Arrange
            var csvContent = "Personnel_Records.Payroll_Number,Personnel_Records.Forenames,Personnel_Records.Surname,Personnel_Records.Date_of_Birth,Personnel_Records.Telephone,Personnel_Records.Mobile,Personnel_Records.Address,Personnel_Records.Address_2,Personnel_Records.Postcode,Personnel_Records.EMail_Home,Personnel_Records.Start_Date\n" +
                             ",John,Doe,1980-01-01,12345,67890,123 Main St,Apt 4,AB12 3CD,john.doe@example.com,2020-01-01\n" +
                             "PI456,Jane,Smith,1985-05-05,54321,09876,456 High St,Suite 7,EF45 6GH,jane.smith@example.com,2021-02-02";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvFileAsync(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().PayrollNumber, Is.EqualTo("PI456"));
        }

        [Test]
        public async Task ParseCsvFileAsync_InvalidDateFormat_ParsesCorrectly()
        {
            // Arrange
            var csvContent = "Personnel_Records.Payroll_Number,Personnel_Records.Forenames,Personnel_Records.Surname,Personnel_Records.Date_of_Birth,Personnel_Records.Telephone,Personnel_Records.Mobile,Personnel_Records.Address,Personnel_Records.Address_2,Personnel_Records.Postcode,Personnel_Records.EMail_Home,Personnel_Records.Start_Date\n" +
                             "PI123,John,Doe,01/01/1980,12345,67890,123 Main St,Apt 4,AB12 3CD,john.doe@example.com,01/01/2020";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _csvParserService.ParseCsvFileAsync(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            
            var employee = result.First();
            Assert.That(employee.DateOfBirth, Is.EqualTo(new DateTime(1980, 1, 1)));
            Assert.That(employee.StartDate, Is.EqualTo(new DateTime(2020, 1, 1)));
        }
    }
} 