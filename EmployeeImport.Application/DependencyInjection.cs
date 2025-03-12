using EmployeeImport.Application.Mappings;
using EmployeeImport.Application.Services;
using EmployeeImport.Application.Validators;
using EmployeeImport.Domain.Entities;
using EmployeeImport.Domain.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeImport.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(typeof(MappingProfile));
            
            // Register validators
            services.AddScoped<IValidator<Employee>, EmployeeValidator>();
            
            // Register application services
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICsvParserService, CsvParserService>();
            
            return services;
        }
    }
} 