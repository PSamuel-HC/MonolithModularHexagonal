using System.Globalization;
using AutoMapper;
using MyModularStore.Employees.Application.DTOs;
using MyModularStore.Employees.Domain;

namespace MyModularStore.Employees.Application.Mapping
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(d => d.HireDate, opt => opt.MapFrom(s => ParseDate(s.HireDate)));

            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(d => d.HireDate, opt => opt.MapFrom(s => ParseDate(s.HireDate)));
        }

        private static DateTime ParseDate(string hireDate)
        {
            if (DateTime.TryParse(hireDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            return default;
        }
    }
}
