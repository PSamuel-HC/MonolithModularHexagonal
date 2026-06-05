using Microsoft.AspNetCore.Mvc;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;

namespace MyModularStore.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderModule orderModule) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders(CancellationToken ct)
        => Ok(await orderModule.GetAllAsync(ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderReadDto>> GetOrder(int id, CancellationToken ct)
    {
        var order = await orderModule.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto dto, CancellationToken ct)
    {
        var result = await orderModule.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto dto, CancellationToken ct)
    {
        var updated = await orderModule.UpdateAsync(id, dto, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken ct)
    {
        var deleted = await orderModule.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }


    [HttpGet("slow-no-cancel")]
    public async Task<IActionResult> SlowWithoutCancellation()
    {
        Console.WriteLine("[NO-CT] Step 1 — starting...");
        await Task.Delay(2000);   // no ct — deaf to cancellation

        Console.WriteLine("[NO-CT] Step 2 — processing...");
        await Task.Delay(2000);

        Console.WriteLine("[NO-CT] Step 3 — sending notifications...");
        await Task.Delay(2000);

        Console.WriteLine("[NO-CT] Step 4 — done! (but nobody is listening)");
        return Ok("Completed");
    }

    [HttpGet("slow-with-cancel")]
    public async Task<IActionResult> SlowWithCancellation(CancellationToken ct)
    {
        try
        {
            Console.WriteLine("[CT] Step 1 — starting...");
            await Task.Delay(2000, ct);

            Console.WriteLine("[CT] Step 2 — processing...");
            await Task.Delay(2000, ct);

            Console.WriteLine("[CT] Step 3 — sending notifications...");
            await Task.Delay(2000, ct);

            Console.WriteLine("[CT] Step 4 — done!");
            return Ok("Completed");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("[CT] *** Client disconnected — stopped cleanly ***");
            return StatusCode(499, "Client disconnected.");
        }
    }


}
