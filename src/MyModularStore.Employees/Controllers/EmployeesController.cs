using Microsoft.AspNetCore.Mvc;
using MyModularStore.Employees.Application.DTOs;
using MyModularStore.Employees.Application.Ports;

namespace MyModularStore.Employees.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(IEmployeeModule employeeModule) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees()
        => Ok(await employeeModule.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var employee = await employeeModule.GetByIdAsync(id);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeDto dto)
    {
        var result = await employeeModule.CreateAsync(dto);
        return CreatedAtAction(nameof(GetEmployee), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDto dto)
    {
        var updated = await employeeModule.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var deleted = await employeeModule.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
