using Microsoft.AspNetCore.Mvc;
using SalesControl.Application.DTOs;
using SalesControl.Application.Interfaces;

namespace SalesControl.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ProductResponse>> List(CancellationToken ct)
    {
        return service.ListAsync(ct);
    }

    [HttpGet("{id:long}")]
    public Task<ProductResponse> GetById(long id, CancellationToken ct)
    {
        return service.GetByIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        ProductRequest request, CancellationToken ct)
    {
        var product = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:long}")]
    public Task<ProductResponse> Update(
        long id, ProductRequest request, CancellationToken ct)
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
