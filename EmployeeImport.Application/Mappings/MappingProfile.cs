using AutoMapper;
using EmployeeImport.Application.DTOs;
using EmployeeImport.Domain.Entities;
using System.Globalization;

namespace EmployeeImport.Application.Mappings
{
    /// <summary>
    /// Mapping profile for AutoMapper
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to DTO mappings
            CreateMap<Employee, EmployeeDto>();
            CreateMap<ImportResult, ImportResultDto>()
                .ForMember(dest => dest.ImportedEmployees, opt => opt.MapFrom(src => src.ImportedEmployees));
            
            // DTO to Entity mappings
            CreateMap<EmployeeDto, Employee>();
            
            // CSV mappings
            CreateMap<string[], Employee>()
                .ForMember(dest => dest.PayrollNumber, opt => opt.MapFrom(src => src[0].Trim()))
                .ForMember(dest => dest.Forenames, opt => opt.MapFrom(src => src[1].Trim()))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src[2].Trim()))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.ParseExact(src[3].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Telephone, opt => opt.MapFrom(src => src[4].Trim()))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src[5].Trim()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src[6].Trim()))
                .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src[7].Trim()))
                .ForMember(dest => dest.Postcode, opt => opt.MapFrom(src => src[8].Trim()))
                .ForMember(dest => dest.EmailHome, opt => opt.MapFrom(src => src[9].Trim()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateTime.ParseExact(src[10].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)));
        }
    }
}