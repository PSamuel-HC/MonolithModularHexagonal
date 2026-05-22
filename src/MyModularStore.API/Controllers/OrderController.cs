using Microsoft.AspNetCore.Mvc;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;

namespace MyModularStore.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OrderController(IOrderModule orderModule) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll() => Ok(await orderModule.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDto>> GetById(int id)
        {
            OrderReadDto? dto = await orderModule.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> Create(OrderCreateDto dto)
        {
            OrderReadDto created = await orderModule.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderUpdateDto dto)
        {
            bool updated = await orderModule.UpdateAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await orderModule.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
