using AutoMapper;
using FluentValidation;
using MyModularStore.Employees.Application.DTOs;
using MyModularStore.Employees.Application.Ports;
using MyModularStore.Employees.Application.Validators;
using MyModularStore.Employees.Domain;
using MyModularStore.Shared.Contracts;

namespace MyModularStore.Employees.Application
{
    public class EmployeeService(
    IEmployeeRepository repository,
    IMapper mapper,
    CreateEmployeeDtoValidator createValidator,
    UpdateEmployeeDtoValidator updateValidator)
    : IEmployeeModule, IEmployeeContract
    {
        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await repository.GetByIdAsync(id);
            return employee is null ? null : mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            await createValidator.ValidateAndThrowAsync(dto);
            var employee = mapper.Map<Employee>(dto);
            await repository.AddAsync(employee);
            return mapper.Map<EmployeeDto>(employee);
        }

        public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto)
        {
            await updateValidator.ValidateAndThrowAsync(dto);
            var employee = await repository.GetByIdAsync(id);
            if (employee is null) return false;
            mapper.Map(dto, employee);
            await repository.UpdateAsync(employee);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await repository.GetByIdAsync(id);
            if (employee is null) return false;
            await repository.DeleteAsync(employee);
            return true;
        }

        public async Task<EmployeeInfo?> GetInfoAsync(int id)
        {
            var employee = await repository.GetByIdAsync(id);
            return employee is null ? null : new EmployeeInfo()
            {
                Id = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Role = employee.Role
            };
        }

        public async Task<bool> ExistsAsync(int id)
            => await repository.GetByIdAsync(id) is not null;
    }
}
