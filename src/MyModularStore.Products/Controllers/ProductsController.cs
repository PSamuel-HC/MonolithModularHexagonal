using Microsoft.AspNetCore.Mvc;
using MyModularStore.Products.Application.DTOs;
using MyModularStore.Products.Application.Ports;

namespace MyModularStore.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductModule productModule) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        => Ok(await productModule.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await productModule.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var result = await productModule.CreateAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        var updated = await productModule.UpdateAsync(id, dto);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await productModule.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
