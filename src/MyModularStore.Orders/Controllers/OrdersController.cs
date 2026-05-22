using Microsoft.AspNetCore.Mvc;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;

namespace MyModularStore.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderModule orderModule) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders()
        => Ok(await orderModule.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderReadDto>> GetOrder(int id)
    {
        var order = await orderModule.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto dto)
    {
        var result = await orderModule.CreateAsync(dto);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto dto)
    {
        var updated = await orderModule.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var deleted = await orderModule.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
