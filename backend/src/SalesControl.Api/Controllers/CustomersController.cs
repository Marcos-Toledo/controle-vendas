using Microsoft.AspNetCore.Mvc;
using SalesControl.Application.DTOs;
using SalesControl.Application.Interfaces;

namespace SalesControl.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController(ICustomerService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<CustomerResponse>> List(CancellationToken ct)
    {
        return service.ListAsync(ct);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public Task<IReadOnlyList<CustomerResponse>> Search(
        [FromQuery] string term, CancellationToken ct)
    {
        return service.SearchAsync(term, ct);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public Task<CustomerResponse> GetById(long id, CancellationToken ct)
    {
        return service.GetByIdAsync(id, ct);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponse>> Create(
        CustomerRequest request, CancellationToken ct)
    {
        var customer = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public Task<CustomerResponse> Update(
        long id, CustomerRequest request, CancellationToken ct)
    {
        return service.UpdateAsync(id, request, ct);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
