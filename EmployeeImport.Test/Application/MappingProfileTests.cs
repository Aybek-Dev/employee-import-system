using AutoMapper;
using EmployeeImport.Application.DTOs;
using EmployeeImport.Application.Mappings;
using EmployeeImport.Domain.Entities;

namespace EmployeeImport.Test.Application
{
    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;
        private MapperConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            
            _mapper = _configuration.CreateMapper();
        }


        [Test]
        public void Map_EmployeeToEmployeeDto_MapsCorrectly()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
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
            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            // Assert
            Assert.That(employeeDto, Is.Not.Null);
            Assert.That(employeeDto.Id, Is.EqualTo(employee.Id));
            Assert.That(employeeDto.PayrollNumber, Is.EqualTo(employee.PayrollNumber));
            Assert.That(employeeDto.Forenames, Is.EqualTo(employee.Forenames));
            Assert.That(employeeDto.Surname, Is.EqualTo(employee.Surname));
            Assert.That(employeeDto.DateOfBirth, Is.EqualTo(employee.DateOfBirth));
            Assert.That(employeeDto.Telephone, Is.EqualTo(employee.Telephone));
            Assert.That(employeeDto.Mobile, Is.EqualTo(employee.Mobile));
            Assert.That(employeeDto.Address, Is.EqualTo(employee.Address));
            Assert.That(employeeDto.Address2, Is.EqualTo(employee.Address2));
            Assert.That(employeeDto.Postcode, Is.EqualTo(employee.Postcode));
            Assert.That(employeeDto.EmailHome, Is.EqualTo(employee.EmailHome));
            Assert.That(employeeDto.StartDate, Is.EqualTo(employee.StartDate));
        }

        [Test]
        public void Map_EmployeeDtoToEmployee_MapsCorrectly()
        {
            // Arrange
            var employeeDto = new EmployeeDto
            {
                Id = 1,
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
            var employee = _mapper.Map<Employee>(employeeDto);

            // Assert
            Assert.That(employee, Is.Not.Null);
            Assert.That(employee.Id, Is.EqualTo(employeeDto.Id));
            Assert.That(employee.PayrollNumber, Is.EqualTo(employeeDto.PayrollNumber));
            Assert.That(employee.Forenames, Is.EqualTo(employeeDto.Forenames));
            Assert.That(employee.Surname, Is.EqualTo(employeeDto.Surname));
            Assert.That(employee.DateOfBirth, Is.EqualTo(employeeDto.DateOfBirth));
            Assert.That(employee.Telephone, Is.EqualTo(employeeDto.Telephone));
            Assert.That(employee.Mobile, Is.EqualTo(employeeDto.Mobile));
            Assert.That(employee.Address, Is.EqualTo(employeeDto.Address));
            Assert.That(employee.Address2, Is.EqualTo(employeeDto.Address2));
            Assert.That(employee.Postcode, Is.EqualTo(employeeDto.Postcode));
            Assert.That(employee.EmailHome, Is.EqualTo(employeeDto.EmailHome));
            Assert.That(employee.StartDate, Is.EqualTo(employeeDto.StartDate));
        }
    }
} 