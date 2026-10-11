using Microsoft.AspNetCore.Mvc;
using SalesControl.Application.DTOs;
using SalesControl.Application.Interfaces;

namespace SalesControl.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<CustomerResponse>> List(CancellationToken ct)
    {
        return service.ListAsync(ct);
    }

    [HttpGet("search")]
    public Task<IReadOnlyList<CustomerResponse>> Search(
        [FromQuery] string term, CancellationToken ct)
    {
        return service.SearchAsync(term, ct);
    }

    [HttpGet("{id:long}")]
    public Task<CustomerResponse> GetById(long id, CancellationToken ct)
    {
        return service.GetByIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(
        CustomerRequest request, CancellationToken ct)
    {
        var customer = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:long}")]
    public Task<CustomerResponse> Update(
        long id, CustomerRequest request, CancellationToken ct)
    {
        return service.UpdateAsync(id, request, ct);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
