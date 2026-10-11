using Microsoft.AspNetCore.Mvc;
using SalesControl.Application.DTOs;
using SalesControl.Application.Interfaces;

namespace SalesControl.Api.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController(ISaleService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<SaleResponse>> List(CancellationToken ct)
    {
        return service.ListAsync(ct);
    }

    [HttpGet("{id:long}")]
    public Task<SaleResponse> GetById(long id, CancellationToken ct)
    {
        return service.GetByIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<SaleResponse>> Create(
        SaleRequest request, CancellationToken ct)
    {
        var sale = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
    }
}
