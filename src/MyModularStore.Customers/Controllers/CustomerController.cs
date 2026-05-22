using Microsoft.AspNetCore.Mvc;
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Customers.Application.Ports;

namespace MyModularStore.Customers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController(ICustomerModule custumerModule) : ControllerBase
    {

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            IEnumerable<CustomerDto> dtos = await custumerModule.GetCustomersAsync();

            return Ok(dtos);
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            CustomerDto customer = await custumerModule.GetOneCustomerAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerCreateDto dto)
        {

            CustomerDto resultDto = await custumerModule.CreateCustomerAsync(dto);

            return CreatedAtAction(nameof(GetCustomers), new { id = resultDto.Id }, resultDto);
        }

        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, CustomerUpdateDto dto)
        {
            var updated = await custumerModule.UpdateCustomerAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var deleted = await custumerModule.DeleteCustomerAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/info")]
        public async Task<ActionResult> GetCustomerInfo(int id)
        {
            var info = await custumerModule.GetInfoAsync(id);
            return info is null ? NotFound() : Ok(info);
        }

        [HttpGet("{id}/exists")]
        public async Task<ActionResult<bool>> CustomerExists(int id)
        => Ok(await custumerModule.ExistsAsync(id));

    }
}
