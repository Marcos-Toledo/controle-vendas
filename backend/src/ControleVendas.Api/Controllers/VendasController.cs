using ControleVendas.Application.DTOs;
using ControleVendas.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleVendas.Api.Controllers;

[ApiController]
[Route("api/vendas")]
public class VendasController(IVendaService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<VendaResponse>> Listar(CancellationToken ct)
    {
        return service.ListarAsync(ct);
    }

    [HttpGet("{id:long}")]
    public Task<VendaResponse> ObterPorId(long id, CancellationToken ct)
    {
        return service.ObterPorIdAsync(id, ct);
    }

    [HttpPost]
    public async Task<ActionResult<VendaResponse>> Criar(
        VendaRequest request, CancellationToken ct)
    {
        var venda = await service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
    }
}
