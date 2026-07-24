using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MyModularStore.Orders.Application.Commands;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Queries;

namespace MyModularStore.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("read-policy")] // all actions
public class OrdersController(IOrderModule orderModule, ISender sender, OrderMetrics metrics) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders(CancellationToken ct)
        => Ok(await sender.Send(new GetAllOrdersQuery(), ct));

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderReadDto>> GetOrder(int id, CancellationToken ct)
    {
        var order = await orderModule.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [EnableRateLimiting("write-policy")] // override
    public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto dto, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var result = await sender.Send(new CreateOrderCommand(dto), ct);

        sw.Stop();
        metrics.OrderCreated();
        metrics.RecordProcessingTime(sw.Elapsed.TotalMilliseconds);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [EnableRateLimiting("write-policy")] // override
    public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto dto, CancellationToken ct)
    {
        var updated = await orderModule.UpdateAsync(id, dto, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [EnableRateLimiting("write-policy")] // override
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken ct)
    {
        var deleted = await orderModule.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("with-customer")]
    public async Task<ActionResult<IEnumerable<OrderWithCustomerReadDto>>> GetOrdersWithCustomer(
        CancellationToken ct)
        => Ok(await orderModule.GetAllWithCustomerAsync(ct));

    [HttpGet("slow-no-cancel")]
    [DisableRateLimiting]
    public async Task<IActionResult> SlowWithoutCancellation()
    {
        ////Console.WriteLine("[NO-CT] Step 1 — starting...");
        ////await Task.Delay(2000);   // no ct — deaf to cancellation

        ////Console.WriteLine("[NO-CT] Step 2 — processing...");
        ////await Task.Delay(2000);

        ////Console.WriteLine("[NO-CT] Step 3 — sending notifications...");
        ////await Task.Delay(2000);

        ////Console.WriteLine("[NO-CT] Step 4 — done! (but nobody is listening)");
        return Ok("Completed");
    }

    [HttpGet("slow-with-cancel")]
    [DisableRateLimiting]
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
