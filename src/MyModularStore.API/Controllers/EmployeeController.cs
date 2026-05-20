using Microsoft.AspNetCore.Mvc;
using MyModularStore.Employees.Application.DTOs;
using MyModularStore.Employees.Application.Ports;

namespace MyModularStore.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeesController(IEmployeeModule employeeModule) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
            => Ok(await employeeModule.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            EmployeeDto? dto = await employeeModule.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
        {
            EmployeeDto created = await employeeModule.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
        {
            bool updated = await employeeModule.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await employeeModule.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
